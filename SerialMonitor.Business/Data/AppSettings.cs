using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business.Data
{
    public class AppSettings
    {
        public string SelectedPort { get; set; }
        public bool AutoconnectEnabled { get; set; }
        public bool AutoswitchEnabled { get; set; }
        public bool WriteMessageToConsole { get; set; } = true;
        public bool WriteCommandToConsole { get; set; } = true;
        public bool WriteResolvedCommandToConsole { get; set; } = false;
        public bool WriteSentBytesToConsole { get; set; } = false;
        public bool ClearConsoleBeforeCommandSent { get; set; }
        public bool ResolveCommandVariables { get; set; } = true;
        public bool HexPrefixEnabled { get; set; }
        public string HexSeparator { get; set; } = " ";
        public int HexFixedColumns { get; set; } = DefaultHexFixedColumns;
        public bool PipeEnabled { get; set; }
        public bool ShowDotForNonPrintableAscii { get; set; }
        public int FontSize { get; set; } = DefaultFontSize;
        public string FontStyle { get; set; }
        public string SendFileLastFolder { get; set; }
        public int SendFileBatchByteSize { get; set; }
        public int SendFileBatchDelayMs { get; set; }
        public bool ShowButtonsTab { get; set; } = true;
        public bool ShowCommandsTab { get; set; } = true;

        public StandaloneAppSettings StandaloneAppSettings { get; set; } = new StandaloneAppSettings();

        [JsonConverter(typeof(StringEnumConverter))]
        public EViewMode ViewMode { get; set; } = EViewMode.Text;

        [JsonConverter(typeof(StringEnumConverter))]
        public EDefaultEndianness DefaultEndianness { get; set; } = EDefaultEndianness.System;
        
        public Dictionary<string, PortSettings> PortsSettingsMap { get; set; } = new Dictionary<string, PortSettings>();

        public List<CustomButtonSetting> CustomButtons { get; set; } = new List<CustomButtonSetting>();
        
        public List<CustomCommandVariable> CustomCommandVariables { get; set; } = new List<CustomCommandVariable>();

        public List<string> CommandHistory { get; set; } = new List<string>();

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

            if (FontSize < DefaultFontSizeMin || FontSize > DefaultFontSizeMax)
            {
                FontSize = DefaultFontSize;
            }

            if (StandaloneAppSettings == null)
            {
                StandaloneAppSettings = new StandaloneAppSettings();
            }
        }

        public const int DefaultHexFixedColumns = 8;
        public const int DefaultHexFixedColumnsMin = 1;
        public const int DefaultHexFixedColumnsMax = 100;
        public const int DefaultFontSizeMin = 6;
        public const int DefaultFontSizeMax = 72;
        public const char VariableStartDelimiter = '%';
        public const char VariableEndDelimiter = '%';
        public const bool IsVariableCaseSensitive = false;
        public const char DataDelimiter = ',';
        public const char DataAttributeDelimiter = '\\';
        public const string BinPrefix = "0b";
        public const string OctPrefix = "0o";
        public const string HexPrefix = "0x";
        private const int DefaultFontSize = 11;
    }
}