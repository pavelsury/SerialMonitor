using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SerialMonitor.Business.Data;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class SettingsManager : NotifyPropertyChanged
    {
        public PortInfo SelectedPort
        {
            get => _selectedPort;
            set
            {
                AppSettings.SelectedPort = value?.Name;
                SetNotifyingProperty(ref _selectedPort, value);
            }
        }

        public bool AutoswitchEnabled
        {
            get => _autoswitchEnabled;
            set
            {
                AppSettings.AutoswitchEnabled = value;
                SetNotifyingValueProperty(ref _autoswitchEnabled, value);
            }
        }

        public EViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                AppSettings.ViewMode = value;
                SetNotifyingValueProperty(ref _viewMode, value);
            }
        }

        public bool WriteMessageToConsole
        {
            get => _writeMessageToConsole;
            set
            {
                AppSettings.WriteMessageToConsole = value;
                SetNotifyingValueProperty(ref _writeMessageToConsole, value);
            }
        }

        public bool WriteCommandToConsole
        {
            get => _writeCommandToConsole;
            set
            {
                AppSettings.WriteCommandToConsole = value;
                SetNotifyingValueProperty(ref _writeCommandToConsole, value);
            }
        }

        public bool HexPrefixEnabled
        {
            get => _hexPrefixEnabled;
            set
            {
                AppSettings.HexPrefixEnabled = value;
                SetNotifyingValueProperty(ref _hexPrefixEnabled, value);
            }
        }

        public string HexSeparator
        {
            get => _hexSeparator;
            set
            {
                AppSettings.HexSeparator = value;
                SetNotifyingProperty(ref _hexSeparator, value);
            }
        }

        public bool PipeEnabled
        {
            get => _pipeEnabled;
            set
            {
                AppSettings.PipeEnabled = value;
                SetNotifyingValueProperty(ref _pipeEnabled, value);
            }
        }

        public bool ShowDotForNonPrintableAscii
        {
            get => _showDotForNonPrintableAscii;
            set
            {
                AppSettings.ShowDotForNonPrintableAscii = value;
                SetNotifyingValueProperty(ref _showDotForNonPrintableAscii, value);
            }
        }

        public int HexFixedColumns
        {
            get => _hexFixedColumns;
            set
            {
                AppSettings.HexFixedColumns = value;
                SetNotifyingValueProperty(ref _hexFixedColumns, value);
            }
        }

        public int FontSize
        {
            get => _fontSize;
            set
            {
                AppSettings.FontSize = value;
                SetNotifyingValueProperty(ref _fontSize, value);
            }
        }

        public AppSettings AppSettings { get; private set; } = new AppSettings();

        public PortSettings GetSettings(string portName) => AppSettings.PortsSettingsMap.GetOrCreate(portName);

        public void ResetSelectedPortSettings()
        {
            var portSettings = new PortSettings();
            AppSettings.PortsSettingsMap[SelectedPort.Name] = portSettings;
            SelectedPort.Settings = portSettings;
        }

        public void Save() => FileHelper.WriteAllTextNoShare(_settingsFilename, JsonConvert.SerializeObject(AppSettings, Formatting.Indented));

        public Task LoadAsync(string settingsFilename, string selectedPort)
        {
            return Task.Run(() =>
            {
                try
                {
                    _settingsFilename = settingsFilename;
                    if (_settingsFilename == null)
                    {
                        var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SerialMonitor2");
                        _settingsFilename = Path.Combine(settingsFolder, "Settings.json");
                        Directory.CreateDirectory(settingsFolder);
                    }

                    if (!File.Exists(_settingsFilename))
                    {
                        return;
                    }

                    var serializationSettings = new JsonSerializerSettings
                    {
                        Error = (s, e) => e.ErrorContext.Handled =
                            e.CurrentObject is AppSettings ||
                            e.CurrentObject is StandaloneAppSettings ||
                            e.CurrentObject is PortSettings
                    };

                    var appSettings = JsonConvert.DeserializeObject<AppSettings>(FileHelper.ReadAllText(_settingsFilename), serializationSettings);
                    if (appSettings != null)
                    {
                        AppSettings = appSettings;
                    }
                }
                catch (JsonException)
                { }
                finally
                {
                    if (!string.IsNullOrEmpty(selectedPort))
                    {
                        AppSettings.SelectedPort = selectedPort;
                    }
                    OnSettingsLoaded();
                }
            });
        }

        protected virtual void OnSettingsLoaded()
        {
            AppSettings.Validate();
            AutoswitchEnabled = AppSettings.AutoswitchEnabled;
            ViewMode = AppSettings.ViewMode;
            WriteMessageToConsole = AppSettings.WriteMessageToConsole;
            WriteCommandToConsole = AppSettings.WriteCommandToConsole;
            HexPrefixEnabled = AppSettings.HexPrefixEnabled;
            HexSeparator = AppSettings.HexSeparator;
            HexFixedColumns = AppSettings.HexFixedColumns;
            PipeEnabled = AppSettings.PipeEnabled;
            ShowDotForNonPrintableAscii = AppSettings.ShowDotForNonPrintableAscii;
            FontSize = AppSettings.FontSize;
        }

        private string _settingsFilename;
        private PortInfo _selectedPort;
        private bool _autoswitchEnabled;
        private EViewMode _viewMode = EViewMode.Text;
        private bool _writeMessageToConsole;
        private bool _writeCommandToConsole;
        private bool _hexPrefixEnabled;
        private string _hexSeparator;
        private int _hexFixedColumns;
        private bool _pipeEnabled;
        private bool _showDotForNonPrintableAscii;
        private int _fontSize;
    }
}
