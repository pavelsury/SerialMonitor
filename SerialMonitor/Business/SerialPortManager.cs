using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class SerialPortManager
    {
        public SerialPortManager(SettingsManager settingsManager, IUsbNotification usbNotification)
        {
            SettingsManager = settingsManager;
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

        public ObservableCollection<PortInfo> Ports { get; set; } = new ObservableCollection<PortInfo>();

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
    }
}
