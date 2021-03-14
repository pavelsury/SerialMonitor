using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace SerialMonitor.Ui
{
    [Guid("decfc908-9657-44ef-beea-8eecc6efceab")]
    public class MainToolWindow : ToolWindowPane
    {
        public MainToolWindow() : base(null)
        {
            Caption = "Serial Monitor 2";
            Content = new SerialMonitorControl();
        }
    }
}
