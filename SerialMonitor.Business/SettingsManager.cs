﻿using System;
using System.IO;
using System.Threading.Tasks;
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
                SetNotifyingProperty(ref _selectedPort, value);
                AppSettings.SelectedPort = _selectedPort?.Name;
            }
        }

        public EViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                SetNotifyingValueProperty(ref _viewMode, value);
                AppSettings.ViewMode = _viewMode;
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
                if (!File.Exists(_settingsFilename))
                {
                    return;
                }

                try
                {
                    var serializationSettings = new JsonSerializerSettings
                    {
                        Error = (s, e) => e.ErrorContext.Handled =
                            e.CurrentObject is AppSettings ||
                            e.CurrentObject is PortSettings
                    };

                    var appSettings = JsonConvert.DeserializeObject<AppSettings>(FileHelper.ReadAllText(_settingsFilename), serializationSettings);
                    if (appSettings != null)
                    {
                        appSettings.Validate();
                        AppSettings = appSettings;
                        ViewMode = AppSettings.ViewMode;
                    }
                }
                catch (JsonException)
                { }
            });
        }

        private readonly string _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SerialMonitor2");
        private readonly string _settingsFilename;

        private PortInfo _selectedPort;
        private EViewMode _viewMode = EViewMode.Text;
    }
}
