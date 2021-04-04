using System;
using System.Windows;

namespace TestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
