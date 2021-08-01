using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace SerialMonitor.Win.Ui.Rules
{
    public class ColorRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string colorText)
            {
                try
                {
                    ColorConverter.ConvertFromString(colorText);
                    return ValidationResult.ValidResult;
                }
                catch (Exception)
                {
                    return InvalidResult;
                }
            }
            return InvalidResult;
        }

        private static readonly ValidationResult InvalidResult = new ValidationResult(false, null);
    }
}