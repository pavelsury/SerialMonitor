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
            SettingsManager = new SettingsManager();
            var fileOutputManager = new FileOutputManager(SettingsManager);
            _consoleManager = new ConsoleManager(SettingsManager, fileOutputManager);
            PortManager = new PortManager(SettingsManager, _consoleManager, _mainThreadRunner, _usbNotification);
            _pipeManager = new PipeManager(PortManager, SettingsManager, _consoleManager, _mainThreadRunner);
        }

        public async Task InitializeAsync()
        {
            await SettingsManager.LoadAsync();
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            _consoleManager.Initialize(consoleWriter);
            _pipeManager.Initialize();
            PortManager.Initialize();
        }

        private void OnShutdownStarted(object sender, EventArgs e)
        {
            SettingsManager.Save();
            PortManager.Dispose();
            _usbNotification.Dispose();
        }

        public SettingsManager SettingsManager { get; }
        public PortManager PortManager { get; }

        private readonly ConsoleManager _consoleManager;
        private readonly PipeManager _pipeManager;
        private readonly MainThreadRunner _mainThreadRunner = new MainThreadRunner(Dispatcher.CurrentDispatcher);
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}
