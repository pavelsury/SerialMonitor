using System.Threading.Tasks;

namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory()
        {
            SettingsManager = new SettingsManager();
            SerialPortManager = new SerialPortManager(SettingsManager, UsbNotification);
        }

        public async Task InitializeAsync() => await SettingsManager.InitializeAsync();

        public void InitializeSync()
        {
            SettingsManager.InitializeSync();
            SerialPortManager.InitializeSync();
        }

        public SerialPortManager SerialPortManager { get; }
        public SettingsManager SettingsManager { get; }
        public UsbNotification UsbNotification { get; } = new UsbNotification();
    }
}
