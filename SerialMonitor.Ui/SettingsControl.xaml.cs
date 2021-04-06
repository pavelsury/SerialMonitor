using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace SerialMonitor.Ui
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        private void OnOutputFilenameTextBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false };
            if (dialog.ShowDialog() == true)
            {
                OutputFilenameTextBox.SetCurrentValue(TextBox.TextProperty, dialog.FileName);
            }
        }

        private void OnComboBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            comboBox.SetCurrentValue(ComboBox.TextProperty, null);
            var bindingExpression = comboBox.GetBindingExpression(ComboBox.TextProperty);
            bindingExpression?.UpdateTarget();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SetCurrentValue(TextBox.TextProperty, null);
            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateTarget();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.FocusedElement is UIElement elementWithFocus)
                {
                    elementWithFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
    }
}
