using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class SerialPortManager : NotifyPropertyChanged
    {
        public SerialPortManager(SettingsManager settingsManager, IMessageLogger messageLogger, IUsbNotification usbNotification)
        {
            _messageLogger = messageLogger;
            SettingsManager = settingsManager;
            _port = new SerialPort();
            usbNotification.DeviceChanged += (s, e) => UpdatePorts();
        }

        public void InitializeSync()
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
            try
            {
                _messageLogger.PrintProcessMessage("Configuring port...");
                
                _port.PortName = SettingsManager.SelectedPort.Name;
                _port.BaudRate = PortSettings.BaudRate;
                _port.DataBits = PortSettings.DataBits;
                _port.Handshake = PortSettings.Handshake;
                _port.Parity = PortSettings.Parity;
                _port.StopBits = PortSettings.StopBits;
                _port.ReadTimeout = PortSettings.ReadTimeoutMs;
                _port.WriteTimeout = PortSettings.WriteTimeoutMs;


                _messageLogger.PrintProcessMessage("Connecting...");
                _port.Open();
            }
            catch (Exception e)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
            
            IsConnected = true;
            _messageLogger.PrintSuccessMessage("Connected!");
        }

        public void Disconnect()
        {
            _messageLogger.PrintProcessMessage($@"{Environment.NewLine}Closing port...");
            
            try
            {
                _port.Close();
            }
            catch (Exception e)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
            
            IsConnected = false;
            _messageLogger.PrintSuccessMessage("Port closed!");
        }

        public void SendText(string text)
        {
            try
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
                _port.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                _messageLogger.PrintErrorMessage(e.Message);
            }
        }

        private bool _isConnected;

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

        private readonly IMessageLogger _messageLogger;
        private readonly SerialPort _port;
    }
}
