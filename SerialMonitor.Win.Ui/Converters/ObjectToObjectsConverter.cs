using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialMonitor.Win.Ui.Converters
{
    public class ObjectToObjectsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var predicate = values[0] != null;
            var falseValue = values[1];
            var trueValue = values[2];

            return BoolToObjectsConverter.Convert(predicate, falseValue, trueValue, null, targetType);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}