using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using SerialMonitor.Business;
using SerialMonitor.Business.Enums;
using SerialMonitor.Win.Business.Factories;

namespace SerialMonitor.Win.Ui
{
    public partial class SerialMonitorControl : UserControl, IConsoleWriter
    {
        public SerialMonitorControl(ModelFactory modelFactory)
        {
            DataContext = modelFactory;
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

        private ModelFactory ModelFactory => (ModelFactory)DataContext;
        private PortManager PortManager => ModelFactory.PortManager;

        private void WriteText(string text, SolidColorBrush brush)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _paragraph.Inlines.Add(new Run(text)
            {
                Foreground = brush,
                FontSize = ModelFactory.SettingsManager.FontSize,
                FontStyle = ModelFactory.SettingsManager.FontStyle
            });

            if (IsAutoscrollEnabled)
            {
                FlowDocumentScrollViewer.ScrollToEnd();
            }

            IsPortConsoleEmpty = false;
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e) => PortManager.ConsoleManager.ClearAll();

        private void OnSendButtonClick(object sender, RoutedEventArgs e) => PortManager.SendText(MessageTextBox.Text);

        private void OnSendFileButtonClick(object sender, RoutedEventArgs e)
        {
            string filename = null;
            var text = MessageTextBox.Text;
            
            if (!string.IsNullOrWhiteSpace(text) && File.Exists(text))
            {
                filename = text;
            }

            if (filename == null)
            {
                var openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = ModelFactory.SettingsManager.AppSettings.SendFileLastFolder
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    filename = openFileDialog.FileName;
                }
            }

            if (filename != null)
            {
                PortManager.SendFileAsync(filename);
            }
        }

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