using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class AppSettings
    {
        public string SelectedPort { get; set; }
        public bool AutoconnectEnabled { get; set; }
        public bool AutoswitchEnabled { get; set; }
        public bool WriteMessageToConsole { get; set; } = true;
        public bool WriteCommandToConsole { get; set; } = true;
        public bool HexPrefixEnabled { get; set; }
        public string HexSeparator { get; set; } = " ";
        public int HexFixedColumns { get; set; } = DefaultHexFixedColumns;
        public bool PipeEnabled { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EViewMode ViewMode { get; set; } = EViewMode.Text;
        public Dictionary<string, PortSettings> PortsSettingsMap { get; set; } = new Dictionary<string, PortSettings>();

        public void Validate()
        {
            PortsSettingsMap?.Values.ForEach(s => s.Validate());
            if (HexSeparator == null)
            {
                HexSeparator = " ";
            }

            if (HexFixedColumns < DefaultHexFixedColumnsMin || HexFixedColumns > DefaultHexFixedColumnsMax)
            {
                HexFixedColumns = DefaultHexFixedColumns;
            }
        }

        public const int DefaultHexFixedColumns = 8;
        public const int DefaultHexFixedColumnsMin = 1;
        public const int DefaultHexFixedColumnsMax = 100;
    }
}