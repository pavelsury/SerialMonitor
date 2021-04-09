using System.Windows;
using System.Windows.Threading;
using SerialMonitor.Business;
using SerialMonitor.Ui;

namespace TestApp
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _modelFactory.InitializeAsync();
            var serialMonitorControl = new SerialMonitorControl { DataContext = _modelFactory.SerialPortManager };
            serialMonitorControl.CreateDefaultResources();
            _modelFactory.SetConsoleWriter(serialMonitorControl);
            new MainWindow { Content = serialMonitorControl }.Show();
        }

        private readonly ModelFactory _modelFactory = new ModelFactory(new AppMainThreadRunner(Dispatcher.CurrentDispatcher));
    }
}
