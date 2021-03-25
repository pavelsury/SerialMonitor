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
                    Ports.AddSorted(new PortInfo
                    {
                        Name = portName,
                        IsAvailable = true,
                        Settings = SettingsManager.GetSettings(portName)
                    });
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
    }
}