using System.Windows;
using System.Windows.Media;
using SerialMonitor.Business;
using SerialMonitor.Win.Business.Helpers;

namespace SerialMonitor.Win.Business
{
    public class WinSettingsManager : SettingsManager
    {
        public FontStyle FontStyle
        {
            get => _fontStyle;
            set
            {
                AppSettings.FontStyle = value.ToString();
                SetNotifyingValueProperty(ref _fontStyle, value);
            }
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                AppSettings.StandaloneAppSettings.BackgroundColor = value.ToString();
                SetNotifyingValueProperty(ref _backgroundColor, value);
            }
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                AppSettings.StandaloneAppSettings.ForegroundColor = value.ToString();
                SetNotifyingValueProperty(ref _foregroundColor, value);
            }
        }

        protected override void OnSettingsLoaded()
        {
            base.OnSettingsLoaded();

            FontStyle = ObjectConverter.FontStyleFromString(AppSettings.FontStyle) ?? FontStyles.Normal;
            AppSettings.FontStyle = FontStyle.ToString();

            BackgroundColor = ObjectConverter.ColorFromString(AppSettings.StandaloneAppSettings.BackgroundColor) ?? Colors.White;
            AppSettings.StandaloneAppSettings.BackgroundColor = BackgroundColor.ToString();
            
            ForegroundColor = ObjectConverter.ColorFromString(AppSettings.StandaloneAppSettings.ForegroundColor) ?? Colors.Black;
            AppSettings.StandaloneAppSettings.ForegroundColor = ForegroundColor.ToString();
        }

        private FontStyle _fontStyle;
        private Color _backgroundColor;
        private Color _foregroundColor;
    }
}