using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SerialMonitor.Business;
using SerialMonitor.Business.Data;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Ui
{
    public partial class SerialMonitorControl : UserControl, IConsoleWriter
    {
        public SerialMonitorControl()
        {
            InitializeComponent();
            FlowDocument.Blocks.Add(_paragraph);
            _dataBrush = (SolidColorBrush)TryFindResource(ResourceKeys.TextBoxTextBrushKey);
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

        public void Write(string text, EMessageType messageType)
        {
            SolidColorBrush brush;

            switch (messageType)
            {
                case EMessageType.Data: brush = _dataBrush; break;
                case EMessageType.Command:
                case EMessageType.Info: brush = Brushes.Green; break;
                case EMessageType.Warning: brush = Brushes.DarkOrange; break;
                case EMessageType.Error: brush = Brushes.Red; break;
                default: throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }

            WriteText(text, brush);
        }

        public void Clear()
        {
            _paragraph.Inlines.Clear();
            IsPortConsoleEmpty = true;
        }

        private PortManager PortManager => (PortManager)DataContext;

        private PortSettings SelectedPortSettings => PortManager.SettingsManager.SelectedPort.Settings;

        private void WriteText(string text, SolidColorBrush brush)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _paragraph.Inlines.Add(new Run(text)
            {
                Foreground = brush,
                FontStyle = PortManager.SettingsManager.AppSettings.FontStyle,
                FontSize = SelectedPortSettings.FontSize
            });

            if (IsAutoscrollEnabled)
            {
                FlowDocumentScrollViewer.ScrollToEnd();
            }

            IsPortConsoleEmpty = false;
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e) => PortManager.ConsoleManager.ClearAll();

        private void OnSendButtonClick(object sender, RoutedEventArgs e) => PortManager.SendText(MessageTextBox.Text);

        private void OnMessageTextBoxKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PortManager.SendText(MessageTextBox.Text);
                MessageTextBox.Text = string.Empty;
            }
        }

        private void OnConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (PortManager.IsConnected)
            {
                PortManager.Disconnect();
            }
            else
            {
                TabControl.SelectedIndex = 0;
                PortManager.Connect();
            }
        }

        private readonly SolidColorBrush _dataBrush;
        private readonly Paragraph _paragraph = new Paragraph();
    }
}