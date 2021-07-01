using System.Windows;
using SerialMonitor.Business;
using SerialMonitor.Ui;

namespace SerialMonitor.App
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            InitializeResourceKeys();
            await _modelFactory.InitializeAsync();
            var serialMonitorControl = new SerialMonitorControl { DataContext = _modelFactory.PortManager };
            _modelFactory.SetConsoleWriter(serialMonitorControl);
            new MainWindow { Content = serialMonitorControl }.Show();
        }

        private static void InitializeResourceKeys()
        {
            ResourceKeys.CheckBoxStyleKey = "CheckBoxStyleKey";
            ResourceKeys.ComboBoxStyleKey = "ComboBoxStyleKey";
            ResourceKeys.ButtonStyleKey = "ButtonStyleKey";
            ResourceKeys.TextBoxStyleKey = "TextBoxStyleKey";
            ResourceKeys.ThemedDialogLabelStyleKey = "ThemedDialogLabelStyleKey";
            ResourceKeys.ThemedDialogHyperlinkStyleKey = "ThemedDialogHyperlinkStyleKey";
            ResourceKeys.WindowPanelTextBrushKey = "WindowPanelTextBrushKey";
            ResourceKeys.ButtonBorderBrushKey = "ButtonBorderBrushKey";
            ResourceKeys.SelectedItemActiveBrushKey = "SelectedItemActiveBrushKey";
            ResourceKeys.SelectedItemActiveTextBrushKey = "SelectedItemActiveTextBrushKey";
            ResourceKeys.ToolWindowTextBrushKey = "ToolWindowTextBrushKey";
            ResourceKeys.TextBoxTextBrushKey = "TextBoxTextBrushKey";
        }

        private readonly ModelFactory _modelFactory = new ModelFactory();
    }
}
