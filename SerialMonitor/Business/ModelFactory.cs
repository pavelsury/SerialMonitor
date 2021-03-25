namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory()
        {
            SettingsManager = new SettingsManager();
            SerialPortManager = new SerialPortManager(SettingsManager, UsbNotification);
        }

        public SerialPortManager SerialPortManager { get; }
        public SettingsManager SettingsManager { get; }
        public UsbNotification UsbNotification { get; } = new UsbNotification();
    }
}
