﻿using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class PortSettings
    {
        public int BaudRate { get; set; } = DefaultBaudRate;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public EReceivingNewline ReceivingNewline { get; set; } = EReceivingNewline.Crlf;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ESendingNewline SendingNewline { get; set; } = ESendingNewline.None;
        
        public int DataBits { get; set; } = DefaultDataBits;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public StopBits StopBits { get; set; } = StopBits.One;
        
        [JsonConverter(typeof(StringEnumConverter))]
        public Handshake Handshake { get; set; } = Handshake.None;
        
        [JsonConverter(typeof(EncodingJsonConverter))]
        [JsonProperty("EncodingCodePage")]
        public Encoding Encoding { get; set; } = Encoding.GetEncoding(0);
        
        public int FontSize { get; set; } = DefaultFontSize;
        
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(Parity.None)]
        public Parity Parity { get; set; } = Parity.None;
        
        public int ReadTimeoutMs { get;set; } = DefaultReadTimeoutMs;
        
        public int WriteTimeoutMs { get; set; } = DefaultWriteTimeoutMs;

        public bool DTREnabled { get; set; } = true;
        
        public bool OutputToFileEnabled { get; set; } = false;

        public string OutputFilename { get; set; }

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

            if (FontSize < 6 || FontSize > 66)
            {
                FontSize = DefaultFontSize;
            }

            if (Encoding == null)
            {
                Encoding = Encoding.GetEncoding(0);
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
        private const int DefaultFontSize = 11;
        private const int DefaultReadTimeoutMs = 500;
        private const int DefaultWriteTimeoutMs = 500;
    }
}