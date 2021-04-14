using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class PortManager : NotifyPropertyChanged, IDisposable
    {
        public PortManager(
            SettingsManager settingsManager,
            IMainThreadRunner mainThreadRunner,
            IMessageLogger messageLogger,
            IUsbNotification usbNotification)
        {
            _mainThreadRunner = mainThreadRunner;
            _messageLogger = messageLogger;
            _usbNotification = usbNotification;
            SettingsManager = settingsManager;
            
            _port = new SerialPort();
            _usbNotification.DeviceChanged += OnUsbDevicesChanged;
        }

        public void Initialize(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;

            foreach (var portName in SettingsManager.AppSettings.PortsSettingsMap.Keys)
            {
                CreatePortInfo(portName, false);
            }

            var selectedPortName = SettingsManager.AppSettings.SelectedPort;
            if (!string.IsNullOrWhiteSpace(selectedPortName))
            {
                SelectedPort = Ports.SingleOrDefault(p => p.Name == selectedPortName) ?? CreatePortInfo(selectedPortName, false);
            }

            UpdatePorts();
        }

        public SettingsManager SettingsManager { get; }

        public ObservableCollection<PortInfo> Ports { get; set; } = new ObservableCollection<PortInfo>();

        public EConnectionState ConnectionState
        {
            get => _connectionState;
            set => SetNotifyingValueProperty(ref _connectionState, value, () =>
            {
                OnPropertyChanged(nameof(IsDisconnected));
                OnPropertyChanged(nameof(IsConnectionChanging));
                OnPropertyChanged(nameof(IsConnectingLong));
                OnPropertyChanged(nameof(IsConnected));
            });
        }

        public bool IsDisconnected => _connectionState == EConnectionState.Disconnected;
        public bool IsConnectionChanging =>
            _connectionState == EConnectionState.ConnectingShort ||
            _connectionState == EConnectionState.ConnectingLong ||
            _connectionState == EConnectionState.DisconnectingGracefully ||
            _connectionState == EConnectionState.DisconnectingByFailure;
        public bool IsConnectingLong => _connectionState == EConnectionState.ConnectingLong;
        public bool IsConnected => _connectionState == EConnectionState.Connected;

        public void Connect()
        {
            if (!IsDisconnected)
            {
                return;
            }

            ConnectionState = EConnectionState.ConnectingShort;

            _portTask = Task.Run(async () =>
            {
                using (var delayCts = new CancellationTokenSource())
                {
                    var delayTask = Task.Delay(750, delayCts.Token).ContinueWith(t => _mainThreadRunner.Run(() =>
                    {
                        if (ConnectionState == EConnectionState.ConnectingShort)
                        {
                            ConnectionState = EConnectionState.ConnectingLong;
                        }
                    }), delayCts.Token);

                    try
                    {
                        _port.PortName = SelectedPort.Name;
                        _port.BaudRate = SelectedPort.Settings.BaudRate;
                        _port.DataBits = SelectedPort.Settings.DataBits;
                        _port.Handshake = SelectedPort.Settings.Handshake;
                        _port.Parity = SelectedPort.Settings.Parity;
                        _port.StopBits = SelectedPort.Settings.StopBits;
                        _port.ReadTimeout = SelectedPort.Settings.ReadTimeoutMs;
                        _port.WriteTimeout = SelectedPort.Settings.WriteTimeoutMs;
                        _port.Open();
                    }
                    catch (Exception e)
                    {
                        _mainThreadRunner.Run(() =>
                        {
                            ConnectionState = EConnectionState.Disconnected;
                            _messageLogger.PrintWarningMessage(GetConnectExceptionMessage(e));
                        });
                        return;
                    }
                    finally
                    {
                        try
                        {
                            delayCts.Cancel();
                            await delayTask;
                        }
                        catch (OperationCanceledException)
                        { }
                    }
                }

                _mainThreadRunner.Run(() =>
                {
                    ConnectionState = EConnectionState.Connected;
                    _messageLogger.PrintInfoMessage($"{SelectedPort.Name} connected!");
                });

                await ReadData();
            });

            string GetConnectExceptionMessage(Exception e) => $"{SelectedPort.Name} connecting failed!{Environment.NewLine}{e.GetType()}: {e.Message}";
        }

        private async Task ReadData()
        {
            const int bufferLength = 10000;
            var buffer = new byte[bufferLength];
            var spareCr = string.Empty;
            string newline;

            switch (SelectedPort.Settings.ReceivingNewline)
            {
                case EReceivingNewline.Crlf: newline = "\r\n"; break;
                case EReceivingNewline.Lf: newline = "\n"; break;
                default: throw new ArgumentOutOfRangeException();
            }

            while (true)
            {
                int bytesCount;
                try
                {
                    bytesCount = await _port.BaseStream.ReadAsync(buffer, 0, bufferLength);
                }
                catch (Exception e)
                {
                    _mainThreadRunner.Run(() =>
                    {
                        if (_connectionState != EConnectionState.DisconnectingGracefully)
                        {
                            _messageLogger.PrintWarningMessage(GetReadExceptionMessage(e));
                        }
                    });
                    return;
                }

                if (bytesCount <= 0)
                {
                    continue;
                }

                var data = spareCr + SelectedPort.Settings.Encoding.GetString(buffer, 0, bytesCount);

                if (SelectedPort.Settings.ReceivingNewline == EReceivingNewline.Crlf && data.EndsWith("\r"))
                {
                    data = data.Remove(data.Length - 1);
                    spareCr = "\r";
                }
                else
                {
                    spareCr = string.Empty;
                }

                if (!string.IsNullOrEmpty(data))
                {
                    ProcessReceivedData(data, newline);
                }
            }

            string GetReadExceptionMessage(Exception e) => $"{SelectedPort.Name} reading failed!{Environment.NewLine}{e.GetType()}: {e.Message}";
        }

        public void Disconnect() => Disconnect(false);

        private void Disconnect(bool isFailure)
        {
            if (!IsConnected)
            {
                return;
            }

            ConnectionState = isFailure ? EConnectionState.DisconnectingByFailure : EConnectionState.DisconnectingGracefully;

            Task.Run(async () =>
            {
                Exception exception = null;

                try
                {
                    _port.Close();
                }
                catch (Exception e)
                {
                    exception = e;
                }

                try
                {
                    await _portTask;
                }
                catch (Exception)
                { }

                _mainThreadRunner.Run(() =>
                {
                    _portTask = Task.CompletedTask;
                    if (!isFailure && exception != null)
                    {
                        _messageLogger.PrintWarningMessage($"{exception.GetType()}: {exception.Message}");
                    }
                    var text = isFailure ? "disconnected unexpectedly!" : "disconnected!";
                    _messageLogger.PrintMessage($"{SelectedPort.Name} {text}{Environment.NewLine}", isFailure ? EMessageType.Error : EMessageType.Info);
                    ConnectionState = EConnectionState.Disconnected;
                });
            });
        }

        public void SendText(string text)
        {
            string newline;
            switch (SelectedPort.Settings.SendingNewline)
            {
                case ESendingNewline.None: newline = string.Empty; break;
                case ESendingNewline.Crlf: newline = "\r\n"; break;
                case ESendingNewline.Lf: newline = "\n"; break;
                default: throw new ArgumentOutOfRangeException();
            }

            var data = Encoding.Convert(Encoding.Default, SelectedPort.Settings.Encoding, Encoding.Default.GetBytes($"{text}{newline}"));
            
            try
            {
                _port.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                _messageLogger.PrintWarningMessage(e.Message);
            }
        }

        public void Dispose()
        {
            if (_port == null)
            {
                return;
            }

            _usbNotification.DeviceChanged -= OnUsbDevicesChanged;

            try
            {
                _port.Dispose();
            }
            catch (Exception)
            { }
            _port = null;
        }

        private PortInfo SelectedPort
        {
            get => SettingsManager.SelectedPort;
            set => SettingsManager.SelectedPort = value;
        }

        private void ProcessReceivedData(string data, string newline)
        {
            var text = data.Replace(newline, "\r");
            lock (_receivedTextLock)
            {
                if (_receivedText == null)
                {
                    _receivedText = text;
                    _mainThreadRunner.Run(WriteReceivedText);
                }
                else
                {
                    _receivedText += text;
                }
            }

            var filename = SelectedPort.Settings.OutputFilename;
            if (!SelectedPort.Settings.OutputToFileEnabled || string.IsNullOrEmpty(filename))
            {
                return;
            }

            try
            {
                File.AppendAllText(filename, data.Replace(newline, Environment.NewLine));
            }
            catch (Exception e)
            {
                _mainThreadRunner.Run(() => _messageLogger.PrintWarningMessage($"Can't write to file: {filename}{Environment.NewLine}Exception: {e.Message}{Environment.NewLine}"));
            }
        }

        private void WriteReceivedText()
        {
            lock (_receivedTextLock)
            {
                if (IsConnected)
                {
                    _consoleWriter.WriteText(_receivedText);
                }
                _receivedText = null;
            }
        }

        private void OnUsbDevicesChanged(object sender, bool e) => UpdatePorts();

        private void UpdatePorts()
        {
            var wasSelectedPortAvailable = SelectedPort?.IsAvailable == true;
            var portNames = SerialPort.GetPortNames();
            
            foreach (var portName in portNames)
            {
                var portInfo = Ports.SingleOrDefault(p => p.Name == portName);
                if (portInfo == null)
                {
                    CreatePortInfo(portName, true);
                }
                else
                {
                    portInfo.IsAvailable = true;
                }
            }

            if (!Ports.Any())
            {
                return;
            }

            foreach (var portInfo in Ports)
            {
                portInfo.IsAvailable = portNames.Any(n => n == portInfo.Name);
            }

            if (SelectedPort == null)
            {
                SelectedPort = Ports.FirstOrDefault(p => p.IsAvailable) ?? Ports.First();
            }

            if (!SelectedPort.IsAvailable && IsConnected)
            {
                Disconnect(true);
                return;
            }

            if (SettingsManager.AppSettings.AutoconnectEnabled && IsDisconnected)
            {
                if (!wasSelectedPortAvailable && SelectedPort.IsAvailable)
                {
                    Connect();
                }
            }
        }

        private PortInfo CreatePortInfo(string portName, bool isAvailable)
        {
            var portInfo = new PortInfo
            {
                Name = portName,
                IsAvailable = isAvailable,
                Settings = SettingsManager.GetSettings(portName)
            };
            Ports.AddSorted(portInfo);
            return portInfo;
        }

        private readonly IMainThreadRunner _mainThreadRunner;
        private readonly IMessageLogger _messageLogger;
        private readonly IUsbNotification _usbNotification;
        private IConsoleWriter _consoleWriter;
        private SerialPort _port;
        private EConnectionState _connectionState;
        private Task _portTask = Task.CompletedTask;
        private string _receivedText;
        private readonly object _receivedTextLock = new object();
    }
}
