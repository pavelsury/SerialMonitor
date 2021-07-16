using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using SerialMonitor.Business;
using SerialMonitor.Business.Factories;
using SerialMonitor.Ui;

namespace SerialMonitor.App
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            AppInfo.IsStandaloneApp = true;
            InitializeResourceKeys();
            await FactoryBuilder.InitializeAsync();
            _modelFactory = FactoryBuilder.ModelFactory;
            _modelFactory.SettingsManager.PropertyChanged += OnSettingsManagerChanged;
            UpdateForegroundResources();

            var serialMonitorControl = new SerialMonitorControl { DataContext = _modelFactory.PortManager };
            _modelFactory.SetConsoleWriter(serialMonitorControl);
            new MainWindow
            {
                DataContext = _modelFactory.PortManager,
                Content = serialMonitorControl
            }.Show();
        }

        private void OnSettingsManagerChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsManager.ForegroundColor))
            {
                UpdateForegroundResources();
            }
        }

        private void UpdateForegroundResources()
        {
            var foregroundBrush = new SolidColorBrush(_modelFactory.SettingsManager.ForegroundColor);
            Current.Resources["AppWindowPanelTextBrush"] = foregroundBrush;
            Current.Resources["AppToolWindowTextBrush"] = foregroundBrush;
        }

        private static void InitializeResourceKeys()
        {
            ResourceKeys.CheckBoxStyleKey = "AppCheckBoxStyle";
            ResourceKeys.ComboBoxStyleKey = "AppComboBoxStyle";
            ResourceKeys.ButtonStyleKey = "AppButtonStyle";
            ResourceKeys.TextBoxStyleKey = "AppTextBoxStyle";
            ResourceKeys.ThemedDialogLabelStyleKey = "AppThemedDialogLabelStyle";
            ResourceKeys.ThemedDialogHyperlinkStyleKey = "AppThemedDialogHyperlinkStyle";
            ResourceKeys.WindowPanelTextBrushKey = "AppWindowPanelTextBrush";
            ResourceKeys.ButtonBorderBrushKey = "AppButtonBorderBrush";
            ResourceKeys.SelectedItemActiveBrushKey = "AppSelectedItemActiveBrush";
            ResourceKeys.SelectedItemActiveTextBrushKey = "AppSelectedItemActiveTextBrush";
            ResourceKeys.ToolWindowTextBrushKey = "AppToolWindowTextBrush";
            ResourceKeys.TextBoxTextBrushKey = "AppTextBoxTextBrush";
            ResourceKeys.CheckBoxMarginKey = "AppCheckBoxMargin";
            ResourceKeys.CheckBoxTextMarginKey = "AppCheckBoxTextMargin";
            ResourceKeys.CheckBoxSmallMarginKey = "AppCheckBoxSmallMargin";
            ResourceKeys.CheckBoxTextSmallMarginKey = "AppCheckBoxTextSmallMargin";
        }

        private ModelFactory _modelFactory;
    }
}
