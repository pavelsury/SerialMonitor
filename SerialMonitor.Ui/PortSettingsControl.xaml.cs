using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using SerialMonitor.Business;

namespace SerialMonitor.Ui
{
    public partial class PortSettingsControl : UserControl
    {
        public PortSettingsControl()
        {
            InitializeComponent();
            BaudRateComboBox.Focus();
        }

        public List<ComboPair> Encodings { get; } = Encoding.GetEncodings().Select(e => new ComboPair
        {
            Value = e.GetEncoding(),
            Text = e.CodePage + " " + e.Name + " - " + e.DisplayName
        }).ToList();

        public List<ComboPair> FontSizes { get; } = Enumerable.Range(PortSettings.DefaultFontSizeMin, PortSettings.DefaultFontSizeMax - PortSettings.DefaultFontSizeMin + 1).Select(i => new ComboPair
        {
            Value = i,
            Text = i.ToString()
        }).ToList();

        public static void OnTextBoxLostFocus(object sender)
        {
            var textBox = (TextBox)sender;
            textBox.SetCurrentValue(TextBox.TextProperty, null);
            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateTarget();
        }

        public static void OnKeyDown(Key key)
        {
            if (key == Key.Enter)
            {
                if (Keyboard.FocusedElement is UIElement elementWithFocus)
                {
                    elementWithFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }

        private SettingsManager SettingsManager => (SettingsManager)DataContext;

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

        public void OnTextBoxLostFocus(object sender, RoutedEventArgs e) => OnTextBoxLostFocus(sender);

        public void OnKeyDown(object sender, KeyEventArgs e) => OnKeyDown(e.Key);

        private void OnResetButtonClick(object sender, RoutedEventArgs e) => SettingsManager.ResetSelectedPortSettings();
    }
}
