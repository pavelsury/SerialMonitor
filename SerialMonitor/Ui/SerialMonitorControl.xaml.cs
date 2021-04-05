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
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            //_port = new SerialPort();
            //_portHandlerTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Application.Current.Dispatcher);
            //_portHandlerTimer.Tick += SerialUpdate;
        }

        public static readonly DependencyProperty IsAutoscrollEnabledProperty = DependencyProperty.Register(
            nameof(IsAutoscrollEnabled), typeof(bool), typeof(SerialMonitorControl), new PropertyMetadata(true));

        public bool IsAutoscrollEnabled
        {
            get => (bool)GetValue(IsAutoscrollEnabledProperty);
            set => SetValue(IsAutoscrollEnabledProperty, value);
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
            Output.AppendText(text, brush, PortSettings.FontSize, PortSettings.FontStyle);

            if (IsAutoscrollEnabled)
            {
                Output.ScrollToEnd();
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            SerialPortManager.SettingsManager.Save();
            //_portHandlerTimer.Stop();
            //_port.Dispose();
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            Output.Clear();
        }

        private void OnSendButtonClick(object sender, RoutedEventArgs e)
        {
            SerialPortManager.SendText(MessageTextBox.Text);
            //try
            //{
            //    var data = Encoding.Convert(
            //        Encoding.Default,
            //        Settings.Encoding,
            //        Encoding.Default.GetBytes(MessageToSend.Text + Settings.SendNewLine));

            //    _port.Write(data, 0, data.Length);
            //}
            //catch (Exception ex)
            //{
            //    PrintErrorMessage(ex.Message);
            //}
        }

        private void OnMessageTextBoxKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SerialPortManager.SendText(MessageTextBox.Text);
                MessageTextBox.Text = string.Empty;
            }
        }

        private void OnConnectDisconnect(object sender, RoutedEventArgs e)
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

        //private void ConfigurePort()
        //{
        //_port.PortName = ComPorts.SelectedItem.ToString();
        //_port.BaudRate = Settings.BaudRate;
        //_port.DataBits = Settings.DataBits;
        //_port.Handshake = Settings.Handshake;
        //_port.Parity = Settings.Parity;
        //_port.StopBits = Settings.StopBits;
        //_port.ReadTimeoutMs = Settings.ReadTimeoutMs;
        //_port.WriteTimeoutMs = Settings.WriteTimeoutMs;
        //}

        //private void SerialUpdate(object e, EventArgs s)
        //{
        //    try
        //    {
        //        var bytesToRead = _port.BytesToRead;

        //        if (bytesToRead > 0)
        //        {
        //            var buffer = new byte[bytesToRead];
        //            _port.Read(buffer, 0, bytesToRead);

        //            var data = Settings.Encoding.GetString(buffer);
        //            Output.AppendText(data.Replace(Settings.ReceiveNewLine, "\r"), Settings.OutputFontSize, Settings.OutputFontStyle);

        //            if (_autoScrollEnabled)
        //            {
        //                Output.ScrollToEnd();
        //            }

        //            if (Settings.OutputToFileEnabled)
        //            {
        //                var file = Settings.RecordFile;

        //                if (!string.IsNullOrEmpty(file) && File.Exists(file))
        //                {
        //                    File.AppendAllText(file, data.Replace(Settings.ReceiveNewLine, Environment.NewLine));
        //                }
        //            }
        //        }
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        PrintErrorMessage(Environment.NewLine + ex.Message);
        //        Disconnect();
        //    }
        //    catch (Exception ex)
        //    {
        //        PrintErrorMessage(Environment.NewLine + ex.Message);
        //    }
        //}

        //private void SettingsOutputControl_Click(object sender, RoutedEventArgs e)
        //{
            //if (Settings.Visibility == Visibility.Visible)
            //{
            //    Settings.Visibility = Visibility.Collapsed;
            //    Output.Visibility = Visibility.Visible;
            //    SettingsOutputControl.Content = "Show Settings";
            //}
            //else if (Settings.Visibility == Visibility.Collapsed)
            //{
            //    Settings.Visibility = Visibility.Visible;
            //    Output.Visibility = Visibility.Collapsed;
            //    SettingsOutputControl.Content = "Show Output";
            //}
        //}

        //private void Connect_Click(object sender, RoutedEventArgs e)
        //{
        //    Settings.Visibility = Visibility.Collapsed;
        //    Output.Visibility = Visibility.Visible;
        //    SettingsOutputControl.Content = "Show Settings";

        //    if (ComPorts.SelectedIndex != -1)
        //    {
        //        try
        //        {
        //            PrintProcessMessage("Configuring port...");
        //            ConfigurePort();

        //            PrintProcessMessage("Connecting...");
        //            _port.Open();

        //            if (Settings.DtrEnable)
        //            {
        //                _port.DtrEnable = true;
        //                _port.DiscardInBuffer();
        //                Thread.Sleep(1000);
        //                _port.DtrEnable = false;
        //            }

        //            ConnectButton.Visibility = Visibility.Collapsed;
        //            DisconnectButton.Visibility = Visibility.Visible;
        //            ReconnectButton.Visibility = Visibility.Visible;
        //            ComPorts.IsEnabled = false;

        //            MessageToSend.IsEnabled = true;
        //            SendButton.IsEnabled = true;

        //            Settings.IsEnabled = false;

        //            PrintSuccessMessage("Connected!");
        //            _portHandlerTimer.Start();
        //        }
        //        catch (Exception ex)
        //        {
        //            PrintErrorMessage(ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        PrintErrorMessage("COM Port not selected!");
        //    }
        //}

        //private void Disconnect_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        PrintProcessMessage(Environment.NewLine + "Closing port...");
        //        Disconnect();
        //        PrintSuccessMessage("Port closed!");
        //    }
        //    catch (Exception ex)
        //    {
        //        PrintErrorMessage(ex.Message);
        //    }
        //}

        //private void Disconnect()
        //{
        //    _portHandlerTimer.Stop();
        //    _port.Close();

        //    ConnectButton.Visibility = Visibility.Visible;
        //    DisconnectButton.Visibility = Visibility.Collapsed;
        //    ReconnectButton.Visibility = Visibility.Collapsed;
        //    ComPorts.IsEnabled = true;
        //    MessageToSend.IsEnabled = false;
        //    SendButton.IsEnabled = false;
        //    Settings.IsEnabled = true;
        //}

        //private void Reconnect_Click(object sender, RoutedEventArgs e)
        //{
        //    Disconnect_Click(null, null);
        //    Connect_Click(null, null);
        //}

        //private void ComPorts_DropDownOpened(object sender, EventArgs e)
        //{
            //string selectedPort = null;
            //if (ComPorts.SelectedIndex != -1)
            //{
            //    selectedPort = ComPorts.SelectedItem.ToString();
            //}
            //ComPorts.Items.Clear();

            //foreach (var portName in SerialPort.GetPortNames())
            //{
            //    ComPorts.Items.Add(portName);
            //}

            //if (selectedPort != null && ComPorts.Items.Contains(selectedPort))
            //{
            //    ComPorts.SelectedItem = selectedPort;
            //}
        //}
    }
}