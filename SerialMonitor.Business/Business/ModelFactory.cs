using System.Threading.Tasks;

namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory(IMainThreadRunner mainThreadRunner)
        {
            SettingsManager = new SettingsManager();
            MessageLogger = new MessageLogger(SettingsManager);
            SerialPortManager = new SerialPortManager(SettingsManager, mainThreadRunner, MessageLogger, UsbNotification);
        }

        public async Task InitializeAsync() => await SettingsManager.InitializeAsync();

        public void InitializeSync()
        {
            UsbNotification.Initialize();
            SettingsManager.InitializeSync();
            SerialPortManager.InitializeSync();
        }

        public void SetConsoleWriter(IConsoleWriter consoleWriter)
        {
            MessageLogger.ConsoleWriter = consoleWriter;
            SerialPortManager.ConsoleWriter = consoleWriter;
        }

        public SerialPortManager SerialPortManager { get; }
        public SettingsManager SettingsManager { get; }
        public MessageLogger MessageLogger { get; }
        public UsbNotification UsbNotification { get; } = new UsbNotification();
    }
}
