using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory(IMainThreadRunner mainThreadRunner)
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += OnShutdownStarted;
            _settingsManager = new SettingsManager();
            _messageLogger = new MessageLogger(_settingsManager);
            SerialPortManager = new SerialPortManager(_settingsManager, mainThreadRunner, _messageLogger, _usbNotification);
        }

        public async Task InitializeAsync()
        {
            await _settingsManager.LoadAsync();
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            _messageLogger.ConsoleWriter = consoleWriter;
            SerialPortManager.Initialize(consoleWriter);
        }

        private void OnShutdownStarted(object sender, EventArgs e)
        {
            _settingsManager.Save();
            SerialPortManager.Dispose();
            _usbNotification.Dispose();
        }

        public SerialPortManager SerialPortManager { get; }

        private readonly SettingsManager _settingsManager;
        private readonly MessageLogger _messageLogger;
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}
