using System.IO.Ports;
using System.Text;
using System.Windows;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public class PortSettings
    {
        public int BaudRate { get; set; } = 9600;
        public EReceivingNewline ReceivingNewline { get; set; } = EReceivingNewline.Crlf;
        public ESendingNewline SendingNewline { get; set; } = ESendingNewline.None;
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;
        public Handshake Handshake { get; set; } = Handshake.None;
        public Encoding Encoding { get; set; } = Encoding.GetEncoding(0);
        public int FontSize { get; set; } = 11;
        public FontStyle FontStyle { get; set; } = FontStyles.Oblique;
        public Parity Parity { get; set; } = Parity.None;
        public int ReadTimeout { get;set; } = 500;
        public int WriteTimeout { get; set; } = 500;
    }
}
