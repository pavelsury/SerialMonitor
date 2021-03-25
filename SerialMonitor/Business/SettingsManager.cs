using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class SettingsManager : NotifyPropertyChanged
    {
        public PortInfo SelectedPort
        {
            get => _selectedPort;
            set => SetNotifyingProperty(ref _selectedPort, value);
        }

        public PortSettings GetSettings(string portName) => _appSettings.PortsSettingsMap.GetOrCreate(portName);

        public Dictionary<Encoding, string> Encodings { get; } = Encoding.GetEncodings().ToDictionary(e => e.GetEncoding(), e => e.CodePage + " " + e.Name + " - " + e.DisplayName);

        public List<int> FontSizes { get; } = Enumerable.Range(6, 67).ToList();

        private PortInfo _selectedPort;
        private readonly AppSettings _appSettings = new AppSettings();
    }
}
