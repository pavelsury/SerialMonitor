using System.Globalization;
using System.Windows.Controls;

namespace SerialMonitor.Ui.Rules
{
    public class IntPositiveRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (value)
            {
                case int v: return ValidateValue(v);
                case string s: return int.TryParse(s, out var val) ? ValidateValue(val) : InvalidResult;
                default: return InvalidResult;
            }
        }

        private static ValidationResult ValidateValue(int value)
        {
            return value > 0 ? ValidationResult.ValidResult : InvalidResult;
        }

        private static readonly ValidationResult InvalidResult = new ValidationResult(false, null);
    }
}
