using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class AppSettings
    {
        public bool AutoconnectEnabled { get; set; }
        public string SelectedPort { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EViewMode ViewMode { get; set; } = EViewMode.Text;
        public Dictionary<string, PortSettings> PortsSettingsMap { get; set; } = new Dictionary<string, PortSettings>();

        public void Validate()
        {
            PortsSettingsMap?.Values.ForEach(s => s.Validate());
        }
    }
}