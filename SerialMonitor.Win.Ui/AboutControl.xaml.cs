using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using SerialMonitor.Business;

namespace SerialMonitor.Win.Ui
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            ScriptsLink = $"https://github.com/pavelsury/SerialMonitor2/releases/download/v{AppInfo.Version}/PipeScripts_v{AppInfo.Version}.zip";
            DownloadLink = $"https://github.com/pavelsury/SerialMonitor2/releases/download/v{AppInfo.Version}/SerialMonitor2_v{AppInfo.Version}.exe";
            DownloadNoFrameworkLink = $"https://github.com/pavelsury/SerialMonitor2/releases/download/v{AppInfo.Version}/SerialMonitor2_v{AppInfo.Version}_without_framework.exe";
            InitializeComponent();
        }

        public string ScriptsLink { get; }
        public string DownloadLink { get; }
        public string DownloadNoFrameworkLink { get; }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
