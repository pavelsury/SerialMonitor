using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using SerialMonitor.Business;
using SerialMonitor.Ui;

namespace SerialMonitor
{
    [Guid("decfc908-9657-44ef-beea-8eecc6efceab")]
    public class ToolWindow : ToolWindowPane
    {
        public ToolWindow(ModelFactory modelFactory) : base(null)
        {
            Caption = "Serial Monitor 2";
            InitializeResourceKeys();
            var serialMonitorControl = new SerialMonitorControl { DataContext = modelFactory.PortManager };
            modelFactory.SetConsoleWriter(serialMonitorControl);
            Content = serialMonitorControl;
        }

        private static void InitializeResourceKeys()
        {
            ResourceKeys.CheckBoxStyleKey = VsResourceKeys.CheckBoxStyleKey;
            ResourceKeys.ComboBoxStyleKey = VsResourceKeys.ComboBoxStyleKey;
            ResourceKeys.ButtonStyleKey = VsResourceKeys.ButtonStyleKey;
            ResourceKeys.TextBoxStyleKey = VsResourceKeys.TextBoxStyleKey;
            ResourceKeys.ThemedDialogLabelStyleKey = VsResourceKeys.ThemedDialogLabelStyleKey;
            ResourceKeys.ThemedDialogHyperlinkStyleKey = VsResourceKeys.ThemedDialogHyperlinkStyleKey;
            ResourceKeys.WindowPanelTextBrushKey = ThemedDialogColors.WindowPanelTextBrushKey;
            ResourceKeys.ButtonBorderBrushKey = CommonControlsColors.ButtonBorderBrushKey;
            ResourceKeys.SelectedItemActiveBrushKey = ThemedDialogColors.SelectedItemActiveBrushKey;
            ResourceKeys.SelectedItemActiveTextBrushKey = ThemedDialogColors.SelectedItemActiveTextBrushKey;
            ResourceKeys.ToolWindowTextBrushKey = EnvironmentColors.ToolWindowTextBrushKey;
            ResourceKeys.TextBoxTextBrushKey = CommonControlsColors.TextBoxTextBrushKey;
        }
    }
}
