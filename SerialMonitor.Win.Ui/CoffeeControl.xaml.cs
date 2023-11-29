using System.Diagnostics;
using System.Windows.Controls;

namespace SerialMonitor.Win.Ui
{
    public partial class CoffeeControl : UserControl
    {
        public CoffeeControl()
        {
            InitializeComponent();
        }

        public string BuyMeACoffeeLink { get; } = "https://www.buymeacoffee.com/serialmonitor";

        private void OnBuyMeACoffeeButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(BuyMeACoffeeLink) { UseShellExecute = true });
        }
    }
}
