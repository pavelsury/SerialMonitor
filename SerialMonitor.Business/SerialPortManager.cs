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
    public class SerialPortManager : NotifyPropertyChanged, IDisposable
    {
        public SerialPortManager(
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
            _port.DataReceived += OnDataReceived;
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
                OnPropertyChanged(nameof(IsConnecting));
                OnPropertyChanged(nameof(IsConnectingLong));
                OnPropertyChanged(nameof(IsConnected));
            });
        }

        public bool IsDisconnected => _connectionState == EConnectionState.NotConnected;
        public bool IsConnecting => _connectionState == EConnectionState.ConnectingShort || _connectionState == EConnectionState.ConnectingLong;
        public bool IsConnectingLong => _connectionState == EConnectionState.ConnectingLong;
        public bool IsConnected => _connectionState == EConnectionState.Connected;

        public void Connect()
        {
            if (IsConnecting || IsConnected)
            {
                return;
            }

            ConnectionState = EConnectionState.ConnectingShort;

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
            }
            catch (Exception e) when (
                e is ArgumentNullException ||
                e is ArgumentOutOfRangeException ||
                e is ArgumentException ||
                e is InvalidOperationException ||
                e is IOException)
            {
                ConnectionState = EConnectionState.NotConnected;
                _messageLogger.PrintErrorMessage(GetErrorMessage(e));
                return;
            }
            catch (Exception)
            {
                ConnectionState = EConnectionState.NotConnected;
                throw;
            }

            Task.Run(() =>
            {
                Exception exception = null;
                
                using (var cts = new CancellationTokenSource())
                {
                    try
                    {
                        Task.Delay(750, cts.Token)
                            .ContinueWith(t => _mainThreadRunner.Run(() =>
                            {
                                if (ConnectionState == EConnectionState.ConnectingShort)
                                {
                                    ConnectionState = EConnectionState.ConnectingLong;
                                }
                            }), cts.Token);

                        _port.Open();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        cts.Cancel();
                    }
                }

                _mainThreadRunner.Run(() =>
                {
                    if (exception == null)
                    {
                        ConnectionState = EConnectionState.Connected;
                        _messageLogger.PrintInfoMessage($"{SelectedPort.Name} connected!");
                        return;
                    }

                    ConnectionState = EConnectionState.NotConnected;

                    if (exception is ArgumentOutOfRangeException ||
                        exception is ArgumentException ||
                        exception is InvalidOperationException ||
                        exception is IOException ||
                        exception is UnauthorizedAccessException)
                    {
                        _messageLogger.PrintErrorMessage(GetErrorMessage(exception));
                        return;
                    } 
                    
                    throw exception;
                });
            });

            string GetErrorMessage(Exception e) => $"{SelectedPort.Name} connecting failed!{Environment.NewLine}{e.Message}";
        }

        public void Disconnect() => Disconnect(false);

        private void Disconnect(bool isFailure)
        {
            if (!IsConnected)
            {
                return;
            }

            try
            {
                _port.Close();
            }
            catch (IOException e)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
            
            ConnectionState = EConnectionState.NotConnected;
            
            _messageLogger.PrintMessage($"{SelectedPort.Name} disconnected!{Environment.NewLine}", isFailure ? EMessageType.Warning : EMessageType.Info);
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
            catch (Exception e) when (
                e is ArgumentNullException ||
                e is InvalidOperationException ||
                e is ArgumentOutOfRangeException ||
                e is ArgumentException ||
                e is TimeoutException)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
        }

        public void Dispose()
        {
            if (_port == null)
            {
                return;
            }

            _usbNotification.DeviceChanged -= OnUsbDevicesChanged;

            if (IsConnected)
            {
                Disconnect(true);
            }
            _port.Dispose();
            _port = null;
        }

        private PortInfo SelectedPort
        {
            get => SettingsManager.SelectedPort;
            set => SettingsManager.SelectedPort = value;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e) => _mainThreadRunner.Run(ReadData);

        private void ReadData()
        {
            if (!IsConnected)
            {
                return;
            }

            byte[] buffer;
            try
            {
                var bytesToRead = _port.BytesToRead;
                if (bytesToRead <= 0)
                {
                    return;
                }

                buffer = new byte[bytesToRead];
                _port.Read(buffer, 0, bytesToRead);
            }
            catch (Exception e) when (
                e is InvalidOperationException ||
                e is ArgumentNullException ||
                e is ArgumentOutOfRangeException ||
                e is ArgumentException ||
                e is TimeoutException)
            {
                _messageLogger.PrintErrorMessage($"{Environment.NewLine}{e.Message}");
                Disconnect(true);
                return;
            }

            string newline;
            switch (SelectedPort.Settings.ReceivingNewline)
            {
                case EReceivingNewline.Crlf: newline = "\r\n"; break;
                case EReceivingNewline.Lf: newline = "\n"; break;
                default: throw new ArgumentOutOfRangeException();
            }

            var data = SelectedPort.Settings.Encoding.GetString(buffer);
            _consoleWriter.WriteText(data.Replace(newline, "\r"));

            if (!SelectedPort.Settings.OutputToFileEnabled)
            {
                return;
            }

            var file = SelectedPort.Settings.OutputFilename;
            if (!string.IsNullOrEmpty(file))
            {
                File.AppendAllText(file, data.Replace(newline, Environment.NewLine));
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
    }
}
