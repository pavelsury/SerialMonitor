using System.Windows;
using SerialMonitor.Business;

namespace TestApp
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _modelFactory.InitializeAsync();
            _modelFactory.InitializeSync();
            new MainWindow { DataContext = _modelFactory }.Show();
            _modelFactory.UsbNotification.Initialize();
        }

        private readonly ModelFactory _modelFactory = new ModelFactory();
    }
}
