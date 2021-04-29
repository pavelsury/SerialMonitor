using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SerialMonitor.Ui
{
    public partial class GeneralSettingsControl : UserControl
    {
        public GeneralSettingsControl()
        {
            InitializeComponent();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e) => PortSettingsControl.OnTextBoxLostFocus(sender);

        private void OnKeyDown(object sender, KeyEventArgs e) => PortSettingsControl.OnKeyDown(e.Key);
    }
}
