using SerialMonitor.Business.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SerialMonitor.Win.Ui.Converters
{
    internal class PortEndiannessToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var portEndianness = (EPortEndianness)values[0];
            var defaultEndianness = (EDefaultEndianness)values[1];
            
            switch (portEndianness)
            {
                case EPortEndianness.Default: return PortEndiannessToStringConverter.GetDefaultText(defaultEndianness);
                case EPortEndianness.Little: return "Little";
                case EPortEndianness.Big: return "Big";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetDefaultText(EDefaultEndianness defaultEndianness)
        {
            bool isLittleEndian;
            switch (defaultEndianness)
            {
                case EDefaultEndianness.System: isLittleEndian = BitConverter.IsLittleEndian; break;
                case EDefaultEndianness.Little: isLittleEndian = true; break;
                case EDefaultEndianness.Big: isLittleEndian = false; break;
                default: throw new ArgumentOutOfRangeException();
            }

            return $"Default ({(isLittleEndian ? "Little" : "Big")})";
        }
    }
}
