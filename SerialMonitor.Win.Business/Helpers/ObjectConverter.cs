using System.Windows;
using System.Windows.Media;

namespace SerialMonitor.Win.Business.Helpers
{
    public static class ObjectConverter
    {
        public static Color? ColorFromString(string text)
        {
            try
            {
                return (Color?)ColorConverter.ConvertFromString(text);
            }
            catch
            {
                return null;
            }
        }

        public static FontStyle? FontStyleFromString(string text)
        {
            try
            {
                return (FontStyle?)_fontStyleConverter.ConvertFromInvariantString(text);
            }
            catch
            {
                return null;
            }
        }

        private static readonly FontStyleConverter _fontStyleConverter = new FontStyleConverter();
    }
}