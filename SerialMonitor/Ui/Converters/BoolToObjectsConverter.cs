using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SerialMonitor.Ui.Converters
{
    public class BoolToObjectsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var predicate = values[0] as bool?;
            var falseValue = values[1];
            var trueValue = values[2];
            var notBoolValue = values.Length == 4 ? values[3] : DependencyProperty.UnsetValue;

            return Convert(predicate, falseValue, trueValue, notBoolValue, targetType);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static object Convert(bool? predicate, object falseValue, object trueValue, object notBoolValue, Type targetType)
        {
            TypeConverter typeConverter;

            switch (targetType)
            {
                case Type v when v == typeof(int): typeConverter = Int32Converter; break;
                case Type v when v == typeof(uint): typeConverter = UInt32Converter; break;
                case Type v when v == typeof(double): typeConverter = DoubleConverter; break;
                case Type v when v == typeof(Brush): typeConverter = BrushConverter; break;
                case Type v when v == typeof(FontWeight): typeConverter = FontWeightConverter; break;
                default: typeConverter = null; break;
            }

            switch (predicate)
            {
                case false: return ConvertValue(falseValue, typeConverter);
                case true: return ConvertValue(trueValue, typeConverter);
                case null: return ConvertValue(notBoolValue, typeConverter);
                default: throw new InvalidOperationException();
            }
        }

        private static object ConvertValue(object value, TypeConverter typeConverter) => typeConverter != null && value is string v ? typeConverter.ConvertFromInvariantString(v) : value;

        private static Int32Converter Int32Converter => _int32Converter ?? (_int32Converter = new Int32Converter());
        private static UInt32Converter UInt32Converter => _uint32Converter ?? (_uint32Converter = new UInt32Converter());
        private static DoubleConverter DoubleConverter => _doubleConverter ?? (_doubleConverter = new DoubleConverter());
        private static BrushConverter BrushConverter => _brushConverter ?? (_brushConverter = new BrushConverter());
        private static FontWeightConverter FontWeightConverter => _fontWeightConverter ?? (_fontWeightConverter = new FontWeightConverter());

        private static Int32Converter _int32Converter;
        private static UInt32Converter _uint32Converter;
        private static DoubleConverter _doubleConverter;
        private static BrushConverter _brushConverter;
        private static FontWeightConverter _fontWeightConverter;
    }
}
