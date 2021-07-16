using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SerialMonitor.Ui
{
    public class TextBoxEx : TextBox
    {
        public TextBoxEx()
        {
            _binding = new Binding(nameof(TextInner))
            {
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            
            SetBinding(TextProperty, _binding);
        }

        public ValidationRule ValidationRule
        {
            get => _validationRule;
            set
            {
                _validationRule = value;
                _binding.ValidationRules.Add(_validationRule);
            }
        }

        public static readonly DependencyProperty TextInnerProperty = DependencyProperty.Register(
            nameof(TextInner), typeof(string), typeof(TextBoxEx), new PropertyMetadata(default(string)));

        public string TextInner
        {
            get => (string)GetValue(TextInnerProperty);
            set => SetValue(TextInnerProperty, value);
        }

        public static readonly DependencyProperty TextExProperty = DependencyProperty.Register(
            nameof(TextEx), typeof(string), typeof(TextBoxEx), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string TextEx
        {
            get => (string)GetValue(TextExProperty);
            set => SetValue(TextExProperty, value);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            UpdateTextProperties();
            base.OnLostFocus(e);
        }        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.FocusedElement is UIElement elementWithFocus)
                {
                    UpdateTextProperties();
                    SelectAll();
                }
            }

            base.OnKeyDown(e);
        }


        private void UpdateTextProperties()
        {
            if (Validation.GetHasError(this))
            {
                SetCurrentValue(TextInnerProperty, TextEx);
                GetBindingExpression(TextProperty)?.UpdateTarget();
            }
            else
            {
                var textInner = TextInner;
                SetCurrentValue(TextExProperty, null);
                SetCurrentValue(TextExProperty, textInner);
                GetBindingExpression(TextExProperty)?.UpdateSource();
            }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBoxEx)d;
            textBox.SetCurrentValue(TextInnerProperty, textBox.TextEx);
            textBox.GetBindingExpression(TextProperty)?.UpdateTarget();
        }

        private readonly Binding _binding;
        private ValidationRule _validationRule;
    }
}