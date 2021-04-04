using System.Threading.Tasks;

namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory()
        {
            SettingsManager = new SettingsManager();
            MessageLogger = new MessageLogger(SettingsManager);
            SerialPortManager = new SerialPortManager(SettingsManager, MessageLogger, UsbNotification);
        }

        public async Task InitializeAsync() => await SettingsManager.InitializeAsync();

        public void InitializeSync()
        {
            UsbNotification.Initialize();
            SettingsManager.InitializeSync();
            SerialPortManager.InitializeSync();
        }

        public void SetConsoleMessageLogger(IMessageLogger consoleMessageLogger) => MessageLogger.ConsoleMessageLogger = consoleMessageLogger;

        public SerialPortManager SerialPortManager { get; }
        public SettingsManager SettingsManager { get; }
        public MessageLogger MessageLogger { get; }
        public UsbNotification UsbNotification { get; } = new UsbNotification();
    }
}
