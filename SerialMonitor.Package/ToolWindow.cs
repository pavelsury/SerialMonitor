using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using SerialMonitor.Common.Ui;

namespace SerialMonitor.Package
{
    [Guid("decfc908-9657-44ef-beea-8eecc6efceab")]
    public class ToolWindow : ToolWindowPane
    {
        public ToolWindow() : base(null)
        {
            Caption = "Serial Monitor 2";
            Content = new SerialMonitorControl();
        }
    }
}
