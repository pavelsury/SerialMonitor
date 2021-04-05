using System.Windows;
using System.Windows.Threading;
using SerialMonitor.Business;

namespace TestApp
{
    public partial class App : Application
    {
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _modelFactory.InitializeAsync();
            _modelFactory.InitializeSync();
            var mainWindow = new MainWindow { DataContext = _modelFactory };
            _modelFactory.SetConsoleWriter(mainWindow.SerialMonitorControl);
            mainWindow.Show();
        }

        private readonly ModelFactory _modelFactory = new ModelFactory(new AppMainThreadRunner(Dispatcher.CurrentDispatcher));
    }
}
