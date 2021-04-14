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
            _messageLogger = new MessageLogger(_settingsManager);
            PortManager = new PortManager(_settingsManager, new MainThreadRunner(Dispatcher.CurrentDispatcher), _messageLogger, _usbNotification);
        }

        public async Task InitializeAsync()
        {
            await _settingsManager.LoadAsync();
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            _messageLogger.ConsoleWriter = consoleWriter;
            PortManager.Initialize(consoleWriter);
        }

        private void OnShutdownStarted(object sender, EventArgs e)
        {
            _settingsManager.Save();
            PortManager.Dispose();
            _usbNotification.Dispose();
        }

        public PortManager PortManager { get; }

        private readonly SettingsManager _settingsManager;
        private readonly MessageLogger _messageLogger;
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}
