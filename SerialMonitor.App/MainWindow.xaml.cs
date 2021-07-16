using System;
using System.Windows;
using System.Windows.Threading;

namespace SerialMonitor.App
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void OnClosed(object sender, EventArgs e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
    }
}
