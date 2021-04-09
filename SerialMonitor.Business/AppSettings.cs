using System.Collections.Generic;

namespace SerialMonitor.Business
{
    public class AppSettings
    {
        public bool AutoconnectEnabled { get; set; }
        public string SelectedPort { get; set; }
        public Dictionary<string, PortSettings> PortsSettingsMap { get; set; } = new Dictionary<string, PortSettings>();
    }
}