using System.ComponentModel;
using SerialMonitor.Business;

namespace SerialMonitor.Win.Business
{
    public class WinConsoleManager : ConsoleManager
    {
        public WinConsoleManager(WinSettingsManager settingsManager, FileOutputManager fileOutputManager) : base(settingsManager, fileOutputManager)
        { }

        protected override void OnSettingsManagerChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnSettingsManagerChanged(sender, e);

            if (e.PropertyName == nameof(WinSettingsManager.FontStyle))
            {
                ReprintAll();
            }
        }
    }
}