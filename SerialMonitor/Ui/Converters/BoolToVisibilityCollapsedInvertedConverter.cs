using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SerialMonitor.Ui.Converters
{
    public class BoolToVisibilityCollapsedInvertedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Collapsed : Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
