using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace Serial_Monitor
{
    [Guid("decfc908-9657-44ef-beea-8eecc6efceab")]
    public class SerialMonitor : ToolWindowPane
    {
        public SerialMonitor() : base(null)
        {
            Caption = "Serial Monitor 2";
            Content = new SerialMonitorControl();
        }
    }
}
