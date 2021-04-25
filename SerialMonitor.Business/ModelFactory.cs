using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory()
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += OnShutdownStarted;
            _settingsManager = new SettingsManager();
            _consoleManager = new ConsoleManager(_settingsManager);
            PortManager = new PortManager(_settingsManager, _consoleManager, new MainThreadRunner(Dispatcher.CurrentDispatcher), _usbNotification);
        }

        public async Task InitializeAsync()
        {
            await _settingsManager.LoadAsync();
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            _consoleManager.Initialize(consoleWriter);
            PortManager.Initialize();
        }

        private void OnShutdownStarted(object sender, EventArgs e)
        {
            _settingsManager.Save();
            PortManager.Dispose();
            _usbNotification.Dispose();
        }

        public PortManager PortManager { get; }

        private readonly SettingsManager _settingsManager;
        private readonly ConsoleManager _consoleManager;
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}
