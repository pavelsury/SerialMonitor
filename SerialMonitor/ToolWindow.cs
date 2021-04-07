using System.Runtime.InteropServices;
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
            var serialMonitorControl = new SerialMonitorControl { DataContext = modelFactory.SerialPortManager };
            modelFactory.SetConsoleWriter(serialMonitorControl);
            Content = serialMonitorControl;
        }
    }
}
