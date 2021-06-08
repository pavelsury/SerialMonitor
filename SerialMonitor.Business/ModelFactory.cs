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
            var fileOutputManager = new FileOutputManager(_settingsManager);
            _consoleManager = new ConsoleManager(_settingsManager, fileOutputManager);
            PortManager = new PortManager(_settingsManager, _consoleManager, _mainThreadRunner, _usbNotification);
            _pipeManager = new PipeManager(PortManager, _settingsManager, _consoleManager, _mainThreadRunner);
        }

        public async Task InitializeAsync()
        {
            await _settingsManager.LoadAsync();
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            _consoleManager.Initialize(consoleWriter);
            _pipeManager.Initialize();
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
        private readonly PipeManager _pipeManager;
        private readonly MainThreadRunner _mainThreadRunner = new MainThreadRunner(Dispatcher.CurrentDispatcher);
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}
