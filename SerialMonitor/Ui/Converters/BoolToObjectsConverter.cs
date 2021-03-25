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
            return Convert(predicate, falseValue, trueValue, notBoolValue, typeConverter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object Convert(bool? predicate, object falseValue, object trueValue, object notBoolValue, TypeConverter typeConverter)
        {
            switch (predicate)
            {
                case false: return ConvertValue(falseValue, typeConverter);
                case true: return ConvertValue(trueValue, typeConverter);
                case null: return ConvertValue(notBoolValue, typeConverter);
                default: throw new InvalidOperationException();
            }
        }

        private static object ConvertValue(object value, TypeConverter typeConverter) => typeConverter != null && value is string v ? typeConverter.ConvertFromInvariantString(v) : value;

        private Int32Converter Int32Converter => _int32Converter ?? (_int32Converter = new Int32Converter());
        private UInt32Converter UInt32Converter => _uint32Converter ?? (_uint32Converter = new UInt32Converter());
        private DoubleConverter DoubleConverter => _doubleConverter ?? (_doubleConverter = new DoubleConverter());
        private BrushConverter BrushConverter => _brushConverter ?? (_brushConverter = new BrushConverter());
        private FontWeightConverter FontWeightConverter => _fontWeightConverter ?? (_fontWeightConverter = new FontWeightConverter());

        private Int32Converter _int32Converter;
        private UInt32Converter _uint32Converter;
        private DoubleConverter _doubleConverter;
        private BrushConverter _brushConverter;
        private FontWeightConverter _fontWeightConverter;
    }
}
