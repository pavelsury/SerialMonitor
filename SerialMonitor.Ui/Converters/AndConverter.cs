using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialMonitor.Ui.Converters
{
    public class AndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                switch (value)
                {
                    case bool b when b == false: return false;
                    case int v when v == 0: return false;
                    case null: return false;
                }
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}