using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace SerialMonitor.Business
{
    public class SettingsManager : NotifyPropertyChanged
    {
        public SettingsManager(IUsbNotification usbNotification)
        {
            usbNotification.DeviceChanged += OnUsbDeviceChanged;
            SelectedBaudRate = 307;
        }

        private void OnUsbDeviceChanged(object sender, bool e)
        {
            var portNames = SerialPort.GetPortNames();
            PortNames.Clear();
            foreach (var portName in portNames)
            {
                PortNames.Add(portName);
            }
        }

        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => SetNotifyingValueProperty(ref _selectedBaudRate, value);
        }

        public IEnumerable<int> BaudRates { get; } = new[]
        {
            110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400,
            56000, 57600, 115200, 128000, 153600, 230400, 256000, 460800, 921600
        };

        public ObservableCollection<string> PortNames { get; set; } = new ObservableCollection<string>();
        
        private int _selectedBaudRate;
    }
}
