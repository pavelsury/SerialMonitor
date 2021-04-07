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

        public void WriteLine(string text, EConsoleTextType consoleTextType)
        {
            SolidColorBrush brush;
            switch (consoleTextType)
            {
                case EConsoleTextType.Process: brush = Brushes.Aqua; break;
                case EConsoleTextType.Success: brush = Brushes.Green; break;
                case EConsoleTextType.Warning: brush = Brushes.Yellow; break;
                case EConsoleTextType.Error: brush = Brushes.Red; break;
                default: throw new ArgumentOutOfRangeException(nameof(consoleTextType), consoleTextType, null);
            }
            WriteText($"{text}{Environment.NewLine}", brush);
        }

        private SerialPortManager SerialPortManager => (SerialPortManager)DataContext;

        private PortSettings PortSettings => SerialPortManager.SettingsManager.SelectedPort.Settings;

        private void WriteText(string text, SolidColorBrush brush)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            PortConsole.AppendText(text, brush, PortSettings.FontSize, PortSettings.FontStyle);

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
    }
}