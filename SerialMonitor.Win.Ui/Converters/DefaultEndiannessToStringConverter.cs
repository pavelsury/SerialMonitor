using SerialMonitor.Business.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialMonitor.Win.Ui.Converters
{
    internal class DefaultEndiannessToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLittleEndian;
            switch ((EDefaultEndianness)value)
            {
                case EDefaultEndianness.System: isLittleEndian = BitConverter.IsLittleEndian; break;
                case EDefaultEndianness.Little: isLittleEndian = true; break;
                case EDefaultEndianness.Big: isLittleEndian = false; break;
                default: throw new ArgumentOutOfRangeException();
            }

            return $"Default ({(isLittleEndian ? "Little" : "Big")})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
