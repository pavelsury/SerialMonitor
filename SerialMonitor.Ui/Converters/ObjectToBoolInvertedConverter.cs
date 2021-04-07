using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialMonitor.Ui.Converters
{
    public class ObjectToBoolInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !ObjectToBoolConverter.ConvertToBool(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}