using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using SerialMonitor.Business;

namespace SerialMonitor.Win.Business.Factories
{
    public class ModelFactory
    {
        public async Task InitializeAsync(string settingsFilename, string selectedPort)
        {
            await SettingsManager.LoadAsync(settingsFilename, selectedPort);
            Dispatcher.CurrentDispatcher.ShutdownStarted += OnShutdownStarted;
            ConsoleManager = new WinConsoleManager(SettingsManager, new FileOutputManager(SettingsManager));
            PortManager = new PortManager(SettingsManager, ConsoleManager, _mainThreadRunner, _usbNotification);
            _pipeManager = new PipeManager(PortManager, SettingsManager, ConsoleManager, _mainThreadRunner);
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
        public PortManager PortManager { get; private set; }
        public WinConsoleManager ConsoleManager { get; private set; }

        private PipeManager _pipeManager;
        private readonly MainThreadRunner _mainThreadRunner = new MainThreadRunner();
        private readonly UsbNotification _usbNotification = new UsbNotification();
    }
}