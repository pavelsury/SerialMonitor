using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Text.Json.Serialization;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business.Data
{
    public class PortSettings
    {
        public int BaudRate { get; set; } = DefaultBaudRate;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EReceivingNewline ReceivingNewline { get; set; } = EReceivingNewline.Crlf;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ESendingNewline SendingNewline { get; set; } = ESendingNewline.None;

        public string SendingCustomNewline { get; set; } = string.Empty;
        
        public int DataBits { get; set; } = DefaultDataBits;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StopBits StopBits { get; set; } = StopBits.One;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Handshake Handshake { get; set; } = Handshake.None;
        
        [JsonPropertyName("EncodingCodePage")]
        public Encoding Encoding { get; set; } = Encoding.ASCII;
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [DefaultValue(Parity.None)]
        public Parity Parity { get; set; } = Parity.None;
        
        public int ReadTimeoutMs { get;set; } = DefaultReadTimeoutMs;
        
        public int WriteTimeoutMs { get; set; } = DefaultWriteTimeoutMs;

        public bool DTREnabled { get; set; } = true;
        
        public bool OutputToFileEnabled { get; set; } = false;

        public string OutputFilename { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EPortEndianness Endianness { get; set; } = EPortEndianness.Default;

        public void Validate()
        {
            if (BaudRate <= 0)
            {
                BaudRate = DefaultBaudRate;
            }

            if (DataBits < 5 || DataBits > 8)
            {
                DataBits = DefaultDataBits;
            }

            if (Encoding == null)
            {
                Encoding = Encoding.ASCII;
            }

            if (ReadTimeoutMs < 0)
            {
                ReadTimeoutMs = DefaultReadTimeoutMs;
            }

            if (WriteTimeoutMs < 0)
            {
                WriteTimeoutMs = DefaultWriteTimeoutMs;
            }
        }

        private const int DefaultBaudRate = 9600;
        private const int DefaultDataBits = 8;
        private const int DefaultReadTimeoutMs = 500;
        private const int DefaultWriteTimeoutMs = 500;
    }
}
