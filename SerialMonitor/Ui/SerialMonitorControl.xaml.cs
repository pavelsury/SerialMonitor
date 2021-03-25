using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SerialMonitor.Ui
{
    public partial class SerialMonitorControl : UserControl
    {
        public SerialMonitorControl()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            _port = new SerialPort();
            _portHandlerTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Application.Current.Dispatcher);
            _portHandlerTimer.Tick += SerialUpdate;
        }

        private void ConfigurePort()
        {
            //_port.PortName = ComPorts.SelectedItem.ToString();
            //_port.BaudRate = Settings.BaudRate;
            //_port.DataBits = Settings.DataBits;
            //_port.Handshake = Settings.Handshake;
            //_port.Parity = Settings.Parity;
            //_port.StopBits = Settings.StopBits;
            //_port.ReadTimeout = Settings.ReadTimeout;
            //_port.WriteTimeout = Settings.WriteTimeout;
        }

        private void PrintColorMessage(string message, SolidColorBrush brush)
        {
            //Output.AppendText(message + Environment.NewLine, brush, Settings.OutputFontSize, Settings.OutputFontStyle);

            //if (_autoScrollEnabled)
            //{
            //    Output.ScrollToEnd();
            //}

            //if (Settings.OutputToFileEnabled)
            //{
            //    var file = Settings.RecordFile;

            //    if (!string.IsNullOrEmpty(file) && File.Exists(file))
            //    {
            //        File.AppendAllText(file, message);
            //    }
            //}
        }

        private void PrintErrorMessage(string message) => PrintColorMessage(message, Brushes.Red);

        private void PrintWarningMessage(string message) => PrintColorMessage(message, Brushes.Yellow);

        private void PrintSuccessMessage(string message) => PrintColorMessage(message, Brushes.Green);

        private void PrintProcessMessage(string message) => PrintColorMessage(message, Brushes.Aqua);

        private void SerialUpdate(object e, EventArgs s)
        {
            try
            {
                //var bytesToRead = _port.BytesToRead;

                //if (bytesToRead > 0)
                //{
                //    var buffer = new byte[bytesToRead];
                //    _port.Read(buffer, 0, bytesToRead);

                //    var data = Settings.Encoding.GetString(buffer);
                //    Output.AppendText(data.Replace(Settings.ReceiveNewLine, "\r"), Settings.OutputFontSize, Settings.OutputFontStyle);

                //    if (_autoScrollEnabled)
                //    {
                //        Output.ScrollToEnd();
                //    }

                //    if (Settings.OutputToFileEnabled)
                //    {
                //        var file = Settings.RecordFile;

                //        if (!string.IsNullOrEmpty(file) && File.Exists(file))
                //        {
                //            File.AppendAllText(file, data.Replace(Settings.ReceiveNewLine, Environment.NewLine));
                //        }
                //    }
                //}
            }
            catch (InvalidOperationException ex)
            {
                PrintErrorMessage(Environment.NewLine + ex.Message);
                Disconnect();
            }
            catch (Exception ex)
            {
                PrintErrorMessage(Environment.NewLine + ex.Message);
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _portHandlerTimer.Stop();
            _port.Dispose();
        }

        private void SettingsOutputControl_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Visibility == Visibility.Visible)
            {
                Settings.Visibility = Visibility.Collapsed;
                Output.Visibility = Visibility.Visible;
                SettingsOutputControl.Content = "Show Settings";
            }
            else if (Settings.Visibility == Visibility.Collapsed)
            {
                Settings.Visibility = Visibility.Visible;
                Output.Visibility = Visibility.Collapsed;
                SettingsOutputControl.Content = "Show Output";
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Collapsed;
            Output.Visibility = Visibility.Visible;
            SettingsOutputControl.Content = "Show Settings";

            //if (ComPorts.SelectedIndex != -1)
            //{
            //    try
            //    {
            //        PrintProcessMessage("Configuring port...");
            //        ConfigurePort();

            //        PrintProcessMessage("Connecting...");
            //        _port.Open();

            //        if (Settings.DtrEnable)
            //        {
            //            _port.DtrEnable = true;
            //            _port.DiscardInBuffer();
            //            Thread.Sleep(1000);
            //            _port.DtrEnable = false;
            //        }

            //        ConnectButton.Visibility = Visibility.Collapsed;
            //        DisconnectButton.Visibility = Visibility.Visible;
            //        ReconnectButton.Visibility = Visibility.Visible;
            //        ComPorts.IsEnabled = false;

            //        MessageToSend.IsEnabled = true;
            //        SendButton.IsEnabled = true;

            //        Settings.IsEnabled = false;

            //        PrintSuccessMessage("Connected!");
            //        _portHandlerTimer.Start();
            //    }
            //    catch (Exception ex)
            //    {
            //        PrintErrorMessage(ex.Message);
            //    }
            //}
            //else
            //{
            //    PrintErrorMessage("COM Port not selected!");
            //}
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintProcessMessage(Environment.NewLine + "Closing port...");
                Disconnect();
                PrintSuccessMessage("Port closed!");
            }
            catch (Exception ex)
            {
                PrintErrorMessage(ex.Message);
            }
        }

        private void Disconnect()
        {
            _portHandlerTimer.Stop();
            _port.Close();

            ConnectButton.Visibility = Visibility.Visible;
            DisconnectButton.Visibility = Visibility.Collapsed;
            ReconnectButton.Visibility = Visibility.Collapsed;
            //ComPorts.IsEnabled = true;
            MessageToSend.IsEnabled = false;
            SendButton.IsEnabled = false;
            Settings.IsEnabled = true;
        }

        private void Reconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect_Click(null, null);
            Connect_Click(null, null);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Output.Clear();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
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

        private void ComPorts_DropDownOpened(object sender, EventArgs e)
        {
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
        }

        private void MessageToSend_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Send_Click(null, null);
                MessageToSend.Text = "";
            }
        }

        private void AutoScrollToggle_Click(object sender, RoutedEventArgs e)
        {
            _autoScrollEnabled = !_autoScrollEnabled;

            if (_autoScrollEnabled)
            {
                AutoScrollToggle.Content = "Disable Auto Scroll";
            }
            else
            {
                AutoScrollToggle.Content = "Enable Auto Scroll";
            }
        }

        private readonly SerialPort _port;
        private readonly DispatcherTimer _portHandlerTimer;
        private bool _autoScrollEnabled = true;
    }
}