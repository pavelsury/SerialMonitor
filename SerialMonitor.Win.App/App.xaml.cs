using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using SerialMonitor.Business;
using SerialMonitor.Business.Helpers;
using SerialMonitor.Win.Business;
using SerialMonitor.Win.Business.Factories;
using SerialMonitor.Win.Ui;

namespace SerialMonitor.Win.App
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            AppInfo.IsStandaloneApp = true;
            InitializeResourceKeys();
            await FactoryBuilder.InitializeAsync(e.Args.GetOptionalArgument("settings_file"));
            _modelFactory = FactoryBuilder.ModelFactory;
            _modelFactory.SettingsManager.PropertyChanged += OnSettingsManagerChanged;
            UpdateForegroundResources();

            var serialMonitorControl = new SerialMonitorControl { DataContext = _modelFactory };
            _modelFactory.SetConsoleWriter(serialMonitorControl);
            new MainWindow
            {
                DataContext = _modelFactory.SettingsManager,
                Content = serialMonitorControl
            }.Show();
        }

        private void OnSettingsManagerChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WinSettingsManager.ForegroundColor))
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
