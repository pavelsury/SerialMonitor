namespace SerialMonitor.Business
{
    public class SerialPortManager
    {
        public SerialPortManager(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }
        
        public SettingsManager SettingsManager { get; }
    }
}