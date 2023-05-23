using System.Windows;

namespace SerialMonitor.Win.Ui.Helpers
{
    public class ComboPair : DependencyObject
    {
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(ComboPair), new PropertyMetadata(default));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ComboPair), new PropertyMetadata(default, (d, o) => ((ComboPair)d).OnTextChanged()));

        protected virtual void OnTextChanged()
        { }
    }
}
