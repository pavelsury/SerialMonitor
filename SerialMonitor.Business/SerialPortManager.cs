using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
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

        public IConsoleWriter ConsoleWriter{ private get; set; }

        public void Initialize()
        {
            foreach (var portName in SettingsManager.AppSettings.PortsSettingsMap.Keys)
            {
                CreatePortInfo(portName, false);
            }

            var selectedPortName = SettingsManager.AppSettings.SelectedPort;
            if (!string.IsNullOrWhiteSpace(selectedPortName))
            {
                SettingsManager.SelectedPort = Ports.SingleOrDefault(p => p.Name == selectedPortName) ?? CreatePortInfo(selectedPortName, false);
            }

            UpdatePorts();
        }

        public SettingsManager SettingsManager { get; }

        public PortSettings PortSettings => SettingsManager.SelectedPort.Settings;

        public ObservableCollection<PortInfo> Ports { get; set; } = new ObservableCollection<PortInfo>();

        public bool IsConnected
        {
            get => _isConnected;
            set => SetNotifyingValueProperty(ref _isConnected, value);
        }

        public void Connect()
        {
            _messageLogger.PrintInfoMessage("Connecting...");
            try
            {
                _port.PortName = SettingsManager.SelectedPort.Name;
                _port.BaudRate = PortSettings.BaudRate;
                _port.DataBits = PortSettings.DataBits;
                _port.Handshake = PortSettings.Handshake;
                _port.Parity = PortSettings.Parity;
                _port.StopBits = PortSettings.StopBits;
                _port.ReadTimeout = PortSettings.ReadTimeoutMs;
                _port.WriteTimeout = PortSettings.WriteTimeoutMs;
                _port.Open();
            }
            catch (Exception e) when (
                e is ArgumentNullException ||
                e is ArgumentOutOfRangeException ||
                e is ArgumentException ||
                e is InvalidOperationException ||
                e is IOException ||
                e is UnauthorizedAccessException)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
            
            IsConnected = true;
            _messageLogger.PrintInfoMessage("Connected!");
        }

        public void Disconnect()
        {
            _messageLogger.PrintInfoMessage("Closing port...");
            
            try
            {
                _port.Close();
            }
            catch (IOException e)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
            
            IsConnected = false;
            _messageLogger.PrintInfoMessage("Port closed!");
        }

        public void SendText(string text)
        {
            string newline;
            switch (PortSettings.SendingNewline)
            {
                case ESendingNewline.None: newline = string.Empty; break;
                case ESendingNewline.Crlf: newline = "\r\n"; break;
                case ESendingNewline.Lf: newline = "\n"; break;
                default: throw new ArgumentOutOfRangeException();
            }

            var data = Encoding.Convert(Encoding.Default, PortSettings.Encoding, Encoding.Default.GetBytes($"{text}{newline}"));
            
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
                Disconnect();
            }
            _port.Dispose();
            _port = null;
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
                Disconnect();
                return;
            }

            string newline;
            switch (PortSettings.ReceivingNewline)
            {
                case EReceivingNewline.Crlf: newline = "\r\n"; break;
                case EReceivingNewline.Lf: newline = "\n"; break;
                default: throw new ArgumentOutOfRangeException();
            }

            var data = PortSettings.Encoding.GetString(buffer);
            ConsoleWriter.WriteText(data.Replace(newline, "\r"));

            if (!PortSettings.OutputToFileEnabled)
            {
                return;
            }

            var file = PortSettings.OutputFilename;
            if (!string.IsNullOrEmpty(file))
            {
                File.AppendAllText(file, data.Replace(newline, Environment.NewLine));
            }
        }

        private void OnUsbDevicesChanged(object sender, bool e) => UpdatePorts();

        private void UpdatePorts()
        {
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

            foreach (var portInfo in Ports)
            {
                portInfo.IsAvailable = portNames.Any(n => n == portInfo.Name);
            }

            if (SettingsManager.SelectedPort == null)
            {
                SettingsManager.SelectedPort = Ports.FirstOrDefault(p => p.IsAvailable) ?? Ports.FirstOrDefault();
            }

            if (IsConnected && SettingsManager.SelectedPort?.IsAvailable == false)
            {
                Disconnect();
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
        private SerialPort _port;
        private bool _isConnected;
    }
}
