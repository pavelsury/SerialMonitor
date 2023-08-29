using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class PortManager : NotifyPropertyChanged, IConnectionStatusProvider, IDisposable
    {
        public PortManager(
            SettingsManager settingsManager,
            ConsoleManager consoleManager,
            IMainThreadRunner mainThreadRunner,
            IUsbNotification usbNotification)
        {
            _settingsManager = settingsManager;
            _mainThreadRunner = mainThreadRunner;
            ConsoleManager = consoleManager;
            _usbNotification = usbNotification;

            _commandVariablesResolver = new CommandVariablesResolver(settingsManager, settingsManager.CustomCommandVariables);
            _serialPort = new SerialPort();
        }

        public void Initialize()
        {
            _dataManager = new DataManager(_settingsManager, ConsoleManager, this, _mainThreadRunner);

            foreach (var portName in _settingsManager.AppSettings.PortsSettingsMap.Keys)
            {
                CreatePortInfo(portName, false);
            }

            var selectedPortName = _settingsManager.AppSettings.SelectedPort;
            if (!string.IsNullOrWhiteSpace(selectedPortName))
            {
                SelectedPort = Ports.SingleOrDefault(p => p.Name == selectedPortName) ?? CreatePortInfo(selectedPortName, false);
            }

            _usbNotification.PortArrived += OnPortArrived;
            _usbNotification.PortRemoved += OnPortRemoved;
            _settingsManager.PropertyChanged += OnSettingsManagerChanged;

            _availablePortNames = SerialPort.GetPortNames().ToHashSet();
            UpdatePorts();
        }

        public ConsoleManager ConsoleManager { get; }

        public ObservableCollection<PortInfo> Ports { get; set; } = new ObservableCollection<PortInfo>();

        public EConnectionStatus ConnectionStatus
        {
            get => _connectionStatus;
            set => SetNotifyingValueProperty(ref _connectionStatus, value, () =>
            {
                OnPropertyChanged(nameof(IsDisconnected));
                OnPropertyChanged(nameof(IsConnectionChanging));
                OnPropertyChanged(nameof(IsConnectingLong));
                OnPropertyChanged(nameof(IsConnected));
                if (_connectionStatus == EConnectionStatus.Disconnected && HandleAutoswitch())
                {
                    HandleAutoconnect();
                }
            });
        }

        public bool IsDisconnected => _connectionStatus == EConnectionStatus.Disconnected;

        public bool IsConnectionChanging =>
            _connectionStatus == EConnectionStatus.ConnectingShort ||
            _connectionStatus == EConnectionStatus.ConnectingLong ||
            _connectionStatus == EConnectionStatus.DisconnectingGracefully ||
            _connectionStatus == EConnectionStatus.DisconnectingByFailure;

        public bool IsConnectingLong => _connectionStatus == EConnectionStatus.ConnectingLong;

        public bool IsConnected => _connectionStatus == EConnectionStatus.Connected;

        public bool IsFileSending
        {
            get => _isFileSending;
            set => SetNotifyingValueProperty(ref _isFileSending, value);
        }

        public void Connect()
        {
            if (!IsDisconnected)
            {
                return;
            }

            ConnectionStatus = EConnectionStatus.ConnectingShort;

            _portTask = Task.Run(async () =>
            {
                using (var delayCts = new CancellationTokenSource())
                {
                    var delayTask = Task.Delay(750, delayCts.Token).ContinueWith(t => _mainThreadRunner.Run(() =>
                    {
                        if (ConnectionStatus == EConnectionStatus.ConnectingShort)
                        {
                            ConnectionStatus = EConnectionStatus.ConnectingLong;
                        }
                    }), delayCts.Token);

                    try
                    {
                        _serialPort.PortName = SelectedPort.Name;
                        _serialPort.BaudRate = SelectedPort.Settings.BaudRate;
                        _serialPort.DataBits = SelectedPort.Settings.DataBits;
                        _serialPort.Handshake = SelectedPort.Settings.Handshake;
                        _serialPort.Parity = SelectedPort.Settings.Parity;
                        _serialPort.StopBits = SelectedPort.Settings.StopBits;
                        _serialPort.DtrEnable = SelectedPort.Settings.DTREnabled;
                        _serialPort.ReadTimeout = SelectedPort.Settings.ReadTimeoutMs;
                        _serialPort.WriteTimeout = SelectedPort.Settings.WriteTimeoutMs;
                        _serialPort.Open();
                    }
                    catch (Exception e)
                    {
                        _mainThreadRunner.Run(() =>
                        {
                            ConnectionStatus = EConnectionStatus.Disconnected;
                            ConsoleManager.PrintWarningMessage(GetConnectExceptionMessage(e));
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
                    ConnectionStatus = EConnectionStatus.Connected;
                    ConsoleManager.PrintInfoMessage($"{SelectedPort.Name} connected!");
                });

                await ReadAsync();
            });

            string GetConnectExceptionMessage(Exception e) => $"{SelectedPort.Name} connecting failed!{Environment.NewLine}{e.GetType()}: {e.Message}";
        }

        public void Disconnect() => Disconnect(false);

        public void SendText(string text)
        {
            if (_settingsManager.AppSettings.ClearConsoleBeforeCommandSent)
            {
                ConsoleManager.ClearAll();
            }

            try
            {
                ConsoleManager.PrintCommand($"Command: {text}");
                
                byte[] data;

                if (_settingsManager.AppSettings.ResolveCommandVariables)
                {
                    string resolvedCommand;
                    (data, resolvedCommand) = GetResolvedDataToSend(text);
                    ConsoleManager.PrintMessage($"Resolved command: {resolvedCommand}", EMessageType.CommandResolved);
                }
                else
                {
                    data = GetDataToSend(text);
                }

                ConsoleManager.PrintCommandBytes("Sent bytes:", data);
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                ConsoleManager.PrintWarningMessage(e.Message);
            }
        }

        public async void SendFileAsync(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            if (_settingsManager.AppSettings.ClearConsoleBeforeCommandSent)
            {
                ConsoleManager.ClearAll();
            }

            IsFileSending = true;

            ConsoleManager.PrintCommand($"Sent file: {filename}");

            _settingsManager.AppSettings.SendFileLastFolder = Path.GetDirectoryName(filename);

            try
            {
                var data = File.ReadAllBytes(filename);
                var batchDelayMs = Math.Max(0, _settingsManager.AppSettings.SendFileBatchDelayMs);
                var batchSize = Math.Max(0, _settingsManager.AppSettings.SendFileBatchByteSize);
                if (batchSize == 0)
                {
                    batchSize = data.Length;
                }

                var batchesCount = Math.DivRem(data.Length, batchSize, out var remainderCount);

                for (var i = 0; i < batchesCount; i++)
                {
                    _serialPort.Write(data, i * batchSize, batchSize);

                    var needToDelay = batchDelayMs > 0 && (i + 1 < batchesCount || remainderCount > 0);
                    if (needToDelay)
                    {
                        await Task.Delay(batchDelayMs);
                    }
                }

                if (remainderCount > 0)
                {
                    _serialPort.Write(data, batchesCount * batchSize, remainderCount);
                }
            }
            catch (Exception e)
            {
                ConsoleManager.PrintWarningMessage(e.Message);
            }
            finally
            {
                IsFileSending = false;
            }
        }

        public void Dispose()
        {
            if (_serialPort == null)
            {
                return;
            }

            _usbNotification.PortArrived -= OnPortArrived;
            _usbNotification.PortRemoved -= OnPortRemoved;

            try
            {
                _serialPort.Dispose();
            }
            catch (Exception)
            { }

            _serialPort = null;
        }

        private void Disconnect(bool isFailure)
        {
            if (!IsConnected)
            {
                return;
            }

            ConnectionStatus = isFailure ? EConnectionStatus.DisconnectingByFailure : EConnectionStatus.DisconnectingGracefully;

            Task.Run(async () =>
            {
                Exception exception = null;

                try
                {
                    _serialPort.Close();
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
                        ConsoleManager.PrintWarningMessage($"{exception.GetType()}: {exception.Message}");
                    }
                    var text = isFailure ? "disconnected unexpectedly!" : "disconnected!";
                    ConsoleManager.PrintMessage($"{SelectedPort.Name} {text}", isFailure ? EMessageType.Error : EMessageType.Info);
                    ConnectionStatus = EConnectionStatus.Disconnected;
                });
            });
        }

        private async Task ReadAsync()
        {
            _dataManager.Clean();
            var buffer = new byte[10000];

            while (true)
            {
                int bytesCount;
                try
                {
                    bytesCount = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    _dataManager.Clean();
                    _mainThreadRunner.Run(() =>
                    {
                        if (ConnectionStatus != EConnectionStatus.DisconnectingGracefully)
                        {
                            ConsoleManager.PrintWarningMessage($"{SelectedPort.Name} reading failed!{Environment.NewLine}{e.GetType()}: {e.Message}");
                        }
                        Disconnect();
                    });
                    return;
                }

                if (bytesCount > 0)
                {
                    _dataManager.ProcessReceivedData(buffer, bytesCount);
                }
            }
        }

        private PortInfo SelectedPort
        {
            get => _settingsManager.SelectedPort;
            set => _settingsManager.SelectedPort = value;
        }

        private void OnPortArrived(object sender, string portName)
        {
            _availablePortNames.Add(portName);
            UpdatePorts();
        }

        private void OnPortRemoved(object sender, string portName)
        {
            _availablePortNames.Remove(portName);
            UpdatePorts();
        }

        private void UpdatePorts()
        {
            var wasSelectedPortAvailable = SelectedPort?.IsAvailable == true;

            foreach (var portName in _availablePortNames)
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
                portInfo.IsAvailable = _availablePortNames.Any(n => n == portInfo.Name);
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

            var portChanged = HandleAutoswitch();
            if (portChanged)
            {
                wasSelectedPortAvailable = false;
            }

            if (!wasSelectedPortAvailable)
            {
                HandleAutoconnect();
            }
        }

        private void HandleAutoconnect()
        {
            if (_settingsManager.AppSettings.AutoconnectEnabled && IsDisconnected && SelectedPort.IsAvailable)
            {
                Connect();
            }
        }

        private bool HandleAutoswitch()
        {
            var oldSelectedPort = SelectedPort;

            if (_settingsManager.AutoswitchEnabled && !SelectedPort.IsAvailable)
            {
                var port = Ports.FirstOrDefault(p => p.IsAvailable);
                if (port != null)
                {
                    SelectedPort = port;
                }
            }

            return SelectedPort != oldSelectedPort;
        }

        private PortInfo CreatePortInfo(string portName, bool isAvailable)
        {
            var portInfo = new PortInfo
            {
                Name = portName,
                IsAvailable = isAvailable,
                Settings = _settingsManager.GetSettings(portName)
            };
            Ports.AddSorted(portInfo);
            return portInfo;
        }

        private byte[] GetDataToSend(string command)
        {
            command += GetSendingNewlineText();
            return ConvertToBytes(command);
        }

        private (byte[] data, string resolvedCommand) GetResolvedDataToSend(string command)
        {
            command = _commandVariablesResolver.ResolveTextVariables(command);

            if (!_commandVariablesResolver.IsEolOverridden(command))
            {
                command += GetSendingNewlineText();
                if (SelectedPort.Settings.SendingNewline == ESendingNewline.Custom)
                {
                    command = _commandVariablesResolver.ResolveTextVariables(command);
                }
            }

            command = _commandVariablesResolver.ResolveEolVariables(command);
            
            var resolvedCommand = command.Replace("\n", @"\n").Replace("\r", @"\r");

            var tokenList = _commandVariablesResolver.ResolveDataVariables(command);

            foreach (var (token, tokenBytes) in tokenList.Where(p => p.tokenBytes != null))
            {
                resolvedCommand = resolvedCommand.Replace(token, $"[{tokenBytes.Length}B]");
            }
                
            return (ConvertToBytes(tokenList), resolvedCommand);
        }

        private string GetSendingNewlineText()
        {
            switch (SelectedPort.Settings.SendingNewline)
            {
                case ESendingNewline.None: return string.Empty;
                case ESendingNewline.Crlf: return "\r\n";
                case ESendingNewline.Lf: return "\n";
                case ESendingNewline.Custom: return SelectedPort.Settings.SendingCustomNewline;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private byte[] ConvertToBytes(string text) => Encoding.Convert(Encoding.Default, SelectedPort.Settings.Encoding, Encoding.Default.GetBytes(text));

        private byte[] ConvertToBytes(List<(string token, byte[] tokenBytes)> tokenList) => tokenList
                .SelectMany(p => p.tokenBytes ?? ConvertToBytes(p.token))
                .ToArray();

        private void OnSettingsManagerChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsManager.AutoswitchEnabled))
            {
                if (_settingsManager.AutoswitchEnabled)
                {
                    UpdatePorts();
                }
            }
        }

        private readonly SettingsManager _settingsManager;
        private readonly IMainThreadRunner _mainThreadRunner;
        private readonly IUsbNotification _usbNotification;
        private SerialPort _serialPort;
        private EConnectionStatus _connectionStatus;
        private DataManager _dataManager;
        private Task _portTask = Task.CompletedTask;
        private bool _isFileSending;
        private HashSet<string> _availablePortNames;
        private readonly CommandVariablesResolver _commandVariablesResolver;
    }
}
