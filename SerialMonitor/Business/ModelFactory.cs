namespace SerialMonitor.Business
{
    public class ModelFactory
    {
        public ModelFactory()
        {
            SettingsManager = new SettingsManager(UsbNotification);
            SerialPortManager = new SerialPortManager(SettingsManager);
        }

        public SerialPortManager SerialPortManager { get; }
        public SettingsManager SettingsManager { get; }
        public UsbNotification UsbNotification { get; } = new UsbNotification();
    }
}
