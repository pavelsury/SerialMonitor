using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SerialMonitor.Ui.Converters
{
    public class BoolToVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case bool b: return GetVisibility(b);
                case int i: return GetVisibility(i != 0);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Visibility GetVisibility(bool isVisible) => isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
}
