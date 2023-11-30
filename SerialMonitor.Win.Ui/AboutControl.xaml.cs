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
            ScriptsDownloadLink = $"https://github.com/pavelsury/SerialMonitor2/releases/download/v{AppInfo.Version}/PipeScripts_v{AppInfo.Version}.zip";
            AppDownloadLink = $"https://github.com/pavelsury/SerialMonitor2/releases/download/v{AppInfo.Version}/SerialMonitor2_v{AppInfo.Version}.exe";
            InitializeComponent();
        }

        public string ScriptsDownloadLink { get; }
        public string AppDownloadLink { get; }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
