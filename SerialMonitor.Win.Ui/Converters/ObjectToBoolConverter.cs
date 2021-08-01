using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialMonitor.Win.Ui.Converters
{
    public class ObjectToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToBool(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static bool ConvertToBool(object value)
        {
            switch (value)
            {
                case int v: return v != 0;
                case object _: return true;
                case null: return false;
            }
        }
    }
}