using System.Collections.Generic;

namespace SerialMonitor.Business
{
    public class AppSettings
    {
        public Dictionary<string, PortSettings> PortsSettingsMap { get; set; } = new Dictionary<string, PortSettings>();
    }
}