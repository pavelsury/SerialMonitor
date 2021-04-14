using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SerialMonitor.Business;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Ui
{
    public partial class SerialMonitorControl : UserControl, IConsoleWriter
    {
        public SerialMonitorControl()
        {
            InitializeComponent();
        }

        public void CreateDefaultResources()
        {
            Resources.Add(Microsoft.VisualStudio.PlatformUI.CommonControlsColors.ButtonBorderBrushKey, new SolidColorBrush(Colors.LightGray));
            Resources.Add(Microsoft.VisualStudio.PlatformUI.ThemedDialogColors.SelectedItemActiveBrushKey, new SolidColorBrush(Colors.LightSkyBlue));
        }

        public static readonly DependencyProperty IsAutoscrollEnabledProperty = DependencyProperty.Register(
            nameof(IsAutoscrollEnabled), typeof(bool), typeof(SerialMonitorControl), new PropertyMetadata(true));

        public bool IsAutoscrollEnabled
        {
            get => (bool)GetValue(IsAutoscrollEnabledProperty);
            set => SetValue(IsAutoscrollEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPortConsoleEmptyProperty = DependencyProperty.Register(
            nameof(IsPortConsoleEmpty), typeof(bool), typeof(SerialMonitorControl), new PropertyMetadata(true));

        public bool IsPortConsoleEmpty
        {
            get => (bool)GetValue(IsPortConsoleEmptyProperty);
            set => SetValue(IsPortConsoleEmptyProperty, value);
        }

        public void WriteText(string text)
        {
            WriteText(text, null);
        }

        public void WriteLine(string text, EMessageType messageType)
        {
            SolidColorBrush brush;
            switch (messageType)
            {
                case EMessageType.Info: brush = Brushes.Green; break;
                case EMessageType.Warning: brush = Brushes.DarkOrange; break;
                case EMessageType.Error: brush = Brushes.Red; break;
                default: throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }

            if (_lastInsertedText != null && !_lastInsertedText.EndsWith("\n"))
            {
                text = $"{Environment.NewLine}{text}";
            }

            WriteText($"{text}{Environment.NewLine}", brush);
        }

        private SerialPortManager SerialPortManager => (SerialPortManager)DataContext;

        private PortSettings SelectedPortSettings => SerialPortManager.SettingsManager.SelectedPort.Settings;

        private void WriteText(string text, SolidColorBrush brush)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (brush == null)
            {
                brush = TryFindResource(Microsoft.VisualStudio.PlatformUI.CommonControlsColors.TextBoxTextBrushKey) as SolidColorBrush ?? Brushes.Black;
            }

            PortConsole.AppendText(text, brush, SelectedPortSettings.FontSize, SelectedPortSettings.FontStyle);
            _lastInsertedText = text;

            if (IsAutoscrollEnabled)
            {
                PortConsole.ScrollToEnd();
            }

            IsPortConsoleEmpty = false;
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            PortConsole.Clear();
            IsPortConsoleEmpty = true;
        }

        private void OnSendButtonClick(object sender, RoutedEventArgs e) => SerialPortManager.SendText(MessageTextBox.Text);

        private void OnMessageTextBoxKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SerialPortManager.SendText(MessageTextBox.Text);
                MessageTextBox.Text = string.Empty;
            }
        }

        private void OnConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (SerialPortManager.IsConnected)
            {
                SerialPortManager.Disconnect();
            }
            else
            {
                TabControl.SelectedIndex = 0;
                SerialPortManager.Connect();
            }
        }

        private string _lastInsertedText;
    }
}