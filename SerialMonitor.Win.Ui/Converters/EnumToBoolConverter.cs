using System;
using System.Windows.Data;

namespace SerialMonitor.Win.Ui.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Enum)value).Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
