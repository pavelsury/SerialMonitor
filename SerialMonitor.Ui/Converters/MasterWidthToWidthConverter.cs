using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Microsoft.VisualStudio.Shell.Interop;

namespace SerialMonitor.Ui.Converters
{
    public class MasterWidthToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var masterWidth = (double)values[0];
            return Math.Max(0, masterWidth - values.Skip(1).Sum(GetWidth));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static double GetWidth(object value)
        {
            switch (value)
            {
                case double v: return v;
                case string s: return double.Parse(s);
                default: throw new ArgumentException();
            }
        }
    }
}