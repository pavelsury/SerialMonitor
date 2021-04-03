using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class SettingsManager : NotifyPropertyChanged
    {
        public SettingsManager()
        {
            _settingsFilename = Path.Combine(_settingsFolder, "Settings.json");
        }

        public async Task InitializeAsync() => await LoadAsync();

        public void InitializeSync()
        { }

        public PortInfo SelectedPort
        {
            get => _selectedPort;
            set
            {
                SetNotifyingProperty(ref _selectedPort, value);
                AppSettings.SelectedPort = _selectedPort?.Name;
            }
        }

        public AppSettings AppSettings { get; private set; } = new AppSettings();

        public PortSettings GetSettings(string portName) => AppSettings.PortsSettingsMap.GetOrCreate(portName);

        public Dictionary<Encoding, string> Encodings { get; } = Encoding.GetEncodings().ToDictionary(e => e.GetEncoding(), e => e.CodePage + " " + e.Name + " - " + e.DisplayName);

        public List<int> FontSizes { get; } = Enumerable.Range(6, 67).ToList();

        public void Save()
        {
            Directory.CreateDirectory(_settingsFolder);
            FileHelper.WriteAllTextNoShare(_settingsFilename, JsonConvert.SerializeObject(AppSettings, Formatting.Indented));
        }

        private Task LoadAsync()
        {
            return Task.Run(() =>
            {
                if (!File.Exists(_settingsFilename))
                {
                    return;
                }

                try
                {
                    var serializationSettings = new JsonSerializerSettings
                    {
                        Error = (s, e) => e.ErrorContext.Handled = e.CurrentObject is PortSettings
                    };

                    var appSettings = JsonConvert.DeserializeObject<AppSettings>(FileHelper.ReadAllText(_settingsFilename), serializationSettings);
                    if (appSettings != null)
                    {
                        appSettings.PortsSettingsMap.Values.ForEach(p => p.Validate());
                        AppSettings = appSettings;
                    }
                }
                catch (JsonException)
                { }
            });
        }

        private readonly string _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SerialMonitor2");
        private readonly string _settingsFilename;

        private PortInfo _selectedPort;
    }
}
