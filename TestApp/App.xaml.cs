using System.Windows;
using SerialMonitor.Business;

namespace TestApp
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            new MainWindow { DataContext = _modelFactory }.Show();
            _modelFactory.UsbNotification.Initialize();
        }

        private readonly ModelFactory _modelFactory = new ModelFactory();
    }
}
