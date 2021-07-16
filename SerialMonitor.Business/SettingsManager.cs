using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class SettingsManager : NotifyPropertyChanged
    {
        public SettingsManager()
        {
            _settingsFilename = Path.Combine(_settingsFolder, "Settings.json");
        }

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

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                AppSettings.StandaloneAppSettings.BackgroundColor = value;
                SetNotifyingValueProperty(ref _backgroundColor, value);
            }
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                AppSettings.StandaloneAppSettings.ForegroundColor = value;
                SetNotifyingValueProperty(ref _foregroundColor, value);
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

        public AppSettings AppSettings { get; private set; } = new AppSettings();

        public PortSettings GetSettings(string portName) => AppSettings.PortsSettingsMap.GetOrCreate(portName);

        public void ResetSelectedPortSettings()
        {
            var portSettings = new PortSettings();
            AppSettings.PortsSettingsMap[SelectedPort.Name] = portSettings;
            SelectedPort.Settings = portSettings;
        }

        public void Save()
        {
            Directory.CreateDirectory(_settingsFolder);
            FileHelper.WriteAllTextNoShare(_settingsFilename, JsonConvert.SerializeObject(AppSettings, Formatting.Indented));
        }

        public Task LoadAsync()
        {
            return Task.Run(() =>
            {
                try
                {
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
                        appSettings.Validate();
                        AppSettings = appSettings;
                    }
                }
                catch (JsonException)
                { }
                finally
                {
                    AutoswitchEnabled = AppSettings.AutoswitchEnabled;
                    ViewMode = AppSettings.ViewMode;
                    WriteMessageToConsole = AppSettings.WriteMessageToConsole;
                    WriteCommandToConsole = AppSettings.WriteCommandToConsole;
                    HexPrefixEnabled = AppSettings.HexPrefixEnabled;
                    HexSeparator = AppSettings.HexSeparator;
                    HexFixedColumns = AppSettings.HexFixedColumns;
                    PipeEnabled = AppSettings.PipeEnabled;
                    BackgroundColor = AppSettings.StandaloneAppSettings.BackgroundColor;
                    ForegroundColor = AppSettings.StandaloneAppSettings.ForegroundColor;
                }
            });
        }

        private readonly string _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SerialMonitor2");
        private readonly string _settingsFilename;

        private PortInfo _selectedPort;
        private bool _autoswitchEnabled;
        private EViewMode _viewMode = EViewMode.Text;
        private bool _writeMessageToConsole;
        private bool _writeCommandToConsole;
        private bool _hexPrefixEnabled;
        private string _hexSeparator;
        private int _hexFixedColumns;
        private bool _pipeEnabled;
        private Color _backgroundColor;
        private Color _foregroundColor;
    }
}
