using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using SerialMonitor.Business;

namespace SerialMonitor.Win.Business.Factories
{
    public class ModelFactory
    {
        public ModelFactory()
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += OnShutdownStarted;
            var fileOutputManager = new FileOutputManager(SettingsManager);
            ConsoleManager = new WinConsoleManager(SettingsManager, fileOutputManager);
            PortManager = new PortManager(SettingsManager, ConsoleManager, _mainThreadRunner, _usbNotification);
            _pipeManager = new PipeManager(PortManager, SettingsManager, ConsoleManager, _mainThreadRunner);
        }

        public async Task InitializeAsync(string settingsFilename = null)
        {
            await SettingsManager.LoadAsync(settingsFilename);
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            ConsoleManager.Initialize(consoleWriter);
            _pipeManager.Initialize();
            PortManager.Initialize();
        }

        private void OnShutdownStarted(object sender, EventArgs e)
        {
            SettingsManager.Save();
            PortManager.Dispose();
            _usbNotification.Dispose();
        }

        public WinSettingsManager SettingsManager { get; } = new WinSettingsManager();
        public PortManager PortManager { get; }
        public WinConsoleManager ConsoleManager { get; }

        private readonly PipeManager _pipeManager;
        private readonly MainThreadRunner _mainThreadRunner = new MainThreadRunner();
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}