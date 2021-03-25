using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using SerialMonitor.Business;

namespace SerialMonitor.Ui
{
    public partial class SettingsControl : UserControl
    {
        public string[] BaudRateValues
        {
            get
            {
                return new[]
                {
                    "110",
                    "300",
                    "600",
                    "1200",
                    "2400",
                    "4800",
                    "9600",
                    "14400",
                    "19200",
                    "28800",
                    "38400",
                    "56000",
                    "57600",
                    "115200",
                    "128000",
                    "153600",
                    "230400",
                    "256000",
                    "460800",
                    "921600"
                };
            }
        }
        public string DefaultBaudRate => "9600";

        public Dictionary<string, string> ReceiveNewLineMap =>
            new Dictionary<string, string>
            {
                { "CR + LF", "\r\n" },
                { "LF", "\n" }
            };

        public string DefaultReceiveNewLine => "CR + LF";

        public Dictionary<string, string> SendNewLineMap =>
            new Dictionary<string, string>
            {
                { "None", "" },
                { "CR + LF", "\r\n" },
                { "LF", "\n" }
            };

        public string DefaultSendNewLine => "None";

        public Dictionary<string, StopBits> StopBitsMap =>
            new Dictionary<string, StopBits>
            {
                { "1", StopBits.One },
                { "1.5", StopBits.OnePointFive },
                { "2", StopBits.Two }
            };

        public string DefaultStopBits => "1";

        public Dictionary<string, Handshake> HandshakeMap => Enum.GetNames(typeof(Handshake)).ToDictionary(handshakeValue => handshakeValue, handshakeValue => (Handshake)Enum.Parse(typeof(Handshake), handshakeValue));
        public string DefaultHandshake => Enum.GetNames(typeof(Handshake))[0];

        public Dictionary<string, Parity> ParityMap => Enum.GetNames(typeof(Parity)).ToDictionary(parityValue => parityValue, parityValue => (Parity)Enum.Parse(typeof(Parity), parityValue));
        public string DefaultParity => Enum.GetNames(typeof(Parity))[0];

        public string[] DataBitsValues => new[] { "5", "6", "7", "8" };

        public string DefaultDataBits => "8";

        public string DefaultReadTimeout => "500";

        public string DefaultWriteTimeout => "500";

        public Dictionary<string, Encoding> EncodingsMap => Encoding.GetEncodings().ToDictionary(encoding => encoding.CodePage + " " + encoding.Name + " - " + encoding.DisplayName, encoding => encoding.GetEncoding());

        public string DefaultEncoding
        {
            get
            {
                var defaultEncoding = Encoding.GetEncoding(0);
                return defaultEncoding.CodePage + " " + defaultEncoding.WebName + " - " + defaultEncoding.EncodingName;
            }
        }

        public string[] OutputFontSizeValues
        {
            get
            {
                var values = new List<string>();

                for (var i = 6; i <= 72; i++)
                {
                    values.Add(i.ToString());
                }

                return values.ToArray();
            }
        }

        public string[] OutputFontStyleValues =>
            new[]
            {
                nameof(FontStyles.Normal),
                nameof(FontStyles.Italic),
                nameof(FontStyles.Oblique)
            };

        public string DefaultOutputFontSize => "11";

        public string DefaultOutputFontStyle => nameof(FontStyles.Oblique);

        //public int BaudRate
        //{
        //    get
        //    {
        //        if (!int.TryParse(BaudRateComboBox.Text, out var baudRate))
        //        {
        //            throw new Exception("Invalid baud rate value!");
        //        }
        //        return baudRate;
        //    }
        //}

        //public string ReceiveNewLine => ReceiveNewLineMap[ReceiveNewLineComboBox.Text];

        //public string SendNewLine => SendNewLineMap[SendNewLineComboBox.Text];

        //public StopBits StopBits => StopBitsMap[StopBitsComboBox.Text];

        //public Handshake Handshake => HandshakeMap[HandshakeComboBox.Text];

        //public Parity Parity => ParityMap[ParityComboBox.Text];

        //public int DataBits => Convert.ToInt32(DataBitsComboBox.Text);

        //public int ReadTimeoutMs
        //{
        //    get
        //    {
        //        if (!int.TryParse(ReadTimeoutTextBox.Text, out var readTimeout))
        //        {
        //            throw new Exception("Invalid read timeout value!");
        //        }
        //        return readTimeout;
        //    }
        //}

        //public int WriteTimeoutMs
        //{
        //    get
        //    {
        //        if (!int.TryParse(WriteTimeoutTextBox.Text, out var writeTimeout))
        //        {
        //            throw new Exception("Invalid write timeout value!");
        //        }
        //        return writeTimeout;
        //    }
        //}

        //public Encoding Encoding => EncodingsMap[EncodingComboBox.Text];

        //public int OutputFontSize => Convert.ToInt32(OutputFontSizeComboBox.Text);

        //public FontStyle OutputFontStyle
        //{
        //    get
        //    {
        //        switch (OutputFontStyleComboBox.Text)
        //        {
        //            case nameof(FontStyles.Normal): return FontStyles.Normal;
        //            case nameof(FontStyles.Italic): return FontStyles.Italic;
        //            case nameof(FontStyles.Oblique): return FontStyles.Oblique;
        //            default: throw new ArgumentOutOfRangeException();
        //        }
        //    }
        //}

        public bool DtrEnable { get; private set;
        }

        public bool OutputToFileEnabled { get; private set;
        }

        public string RecordFile => !OutputToFileEnabled ? null : RecordFilePathTextBox.Text;

        public SettingsControl()
        {
            InitializeComponent();
            Reset();
        }

        public void Reset()
        {
            //foreach (var rate in BaudRateValues)
            //{
            //    BaudRateComboBox.Items.Add(rate);
            //}
            //BaudRateComboBox.SelectedItem = DefaultBaudRate;

            //foreach (var newLine in ReceiveNewLineMap.Keys)
            //{
            //    ReceiveNewLineComboBox.Items.Add(newLine);
            //}
            //ReceiveNewLineComboBox.SelectedItem = DefaultReceiveNewLine;

            //foreach (var newLine in SendNewLineMap.Keys)
            //{
            //    SendNewLineComboBox.Items.Add(newLine);
            //}
            //SendNewLineComboBox.SelectedItem = DefaultSendNewLine;

            //foreach (var dataBits in DataBitsValues)
            //{
            //    DataBitsComboBox.Items.Add(dataBits);
            //}
            //DataBitsComboBox.SelectedItem = DefaultDataBits;

            //foreach (var stopBits in StopBitsMap.Keys)
            //{
            //    StopBitsComboBox.Items.Add(stopBits);
            //}
            //StopBitsComboBox.SelectedItem = DefaultStopBits;

            //foreach (var encoding in EncodingsMap.Keys)
            //{
            //    EncodingComboBox.Items.Add(encoding);
            //}
            //EncodingComboBox.SelectedItem = DefaultEncoding;

            //foreach (var fontSize in OutputFontSizeValues)
            //{
            //    OutputFontSizeComboBox.Items.Add(fontSize);
            //}
            //OutputFontSizeComboBox.SelectedItem = DefaultOutputFontSize;

            //foreach (var fontStyle in OutputFontStyleValues)
            //{
            //    OutputFontStyleComboBox.Items.Add(fontStyle);
            //}
            //OutputFontStyleComboBox.SelectedItem = DefaultOutputFontStyle;

            //foreach (var handshakeValue in HandshakeMap.Keys)
            //{
            //    HandshakeComboBox.Items.Add(handshakeValue);
            //}
            //HandshakeComboBox.SelectedItem = DefaultHandshake;

            //foreach (var parityValue in ParityMap.Keys)
            //{
            //    ParityComboBox.Items.Add(parityValue);
            //}
            //ParityComboBox.SelectedItem = DefaultParity;

            //ReadTimeoutTextBox.Text = DefaultReadTimeout;
            //WriteTimeoutTextBox.Text = DefaultWriteTimeout;

            //DtrEnable = true;
            //OutputToFileEnabled = false;
        }

        private PortSettings PortSettings => (PortSettings)DataContext;

        private void RecordFilePathTextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog { Multiselect = false };
            dialog.ShowDialog();
            RecordFilePathTextBox.Text = dialog.FileName;
        }

        private void DtrToggle_Click(object sender, RoutedEventArgs e)
        {
            DtrEnable = !DtrEnable;

            DtrToggle.Content = DtrEnable ? "Disable Data Terminal Ready (DTR)" : "Enable Data Terminal Ready (DTR)";
        }

        private void OutputToFileToggle_Click(object sender, RoutedEventArgs e)
        {
            OutputToFileEnabled = !OutputToFileEnabled;

            if (OutputToFileEnabled)
            {
                OutputToFileToggle.Content = "Disable output to file";
                RecordFilePathTextBox.IsEnabled = true;
            }
            else
            {
                OutputToFileToggle.Content = "Enable output to file";
                RecordFilePathTextBox.IsEnabled = false;
            }
        }

        //private void EnablePipeIpcToggle_Click(object sender, RoutedEventArgs e)
        //{
        //    _enablePipeIpc = !_enablePipeIpc;
        //    PipeIpcToggle.Content = _enablePipeIpc ? "Disable pipe IPC" : "Enable pipe IPC";
        //}

        //private bool _enablePipeIpc;
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
