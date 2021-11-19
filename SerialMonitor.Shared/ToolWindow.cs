using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using SerialMonitor.Win.Ui;
using SerialMonitor.Win.Ui.Factories;

namespace SerialMonitor
{
    [Guid("decfc908-9657-44ef-beea-8eecc6efceab")]
    public class ToolWindow : ToolWindowPane
    {
        public ToolWindow(UiFactory uiFactory) : base(null)
        {
            Caption = "Serial Monitor 2";
            InitializeResourceKeys();
            Content = uiFactory.SerialMonitorControl;
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
