using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialMonitor.Business.Enums;
using SerialMonitor.Business.Helpers;

namespace SerialMonitor.Business
{
    public class DataItem
    {
        public DataItem(byte[] data, SettingsManager settingsManager)
        {
            MessageType = EMessageType.Data;
            Data = new List<byte>(data);
            _consoleText = settingsManager.SelectedPort.Settings.Encoding.GetString(data, 0, data.Length);
            _consoleTextDotted = GetDottedText(data, settingsManager);
            HexData = data.ToHexStringArray().ToList();
        }

        public DataItem(string message, EMessageType messageType)
        {
            if (messageType == EMessageType.Data)
            {
                throw new ArgumentOutOfRangeException();
            }

            _consoleText = message;
            MessageType = messageType;
        }

        public DataItem(string message, byte[] bytes)
        {
            _consoleText = message;
            MessageType = EMessageType.CommandBytes;
            HexData = bytes.ToHexStringArray().ToList();
        }

        public EMessageType MessageType { get; }

        public List<byte> Data { get; }

        public List<string> HexData { get; }

        public string Text => _stringBuilder?.ToString() ?? _consoleText;
        public string TextDotted => _stringBuilderDotted?.ToString() ?? _consoleTextDotted;

        public void Append(DataItem dataItem)
        {
            Data?.AddRange(dataItem.Data);
            HexData?.AddRange(dataItem.HexData);

            if (_stringBuilder == null)
            {
                _stringBuilder = new StringBuilder(_consoleText);
            }

            if (_stringBuilderDotted == null)
            {
                _stringBuilderDotted = new StringBuilder(_consoleTextDotted);
            }

            _stringBuilder.Append(dataItem.Text);
            _stringBuilderDotted.Append(dataItem.TextDotted);
        }

        public void Freeze()
        {
            if (_stringBuilder != null)
            {
                _consoleText = _stringBuilder.ToString();
                _stringBuilder.Clear();
                _stringBuilder = null;
            }

            if (_stringBuilderDotted != null)
            {
                _consoleTextDotted = _stringBuilderDotted.ToString();
                _stringBuilderDotted.Clear();
                _stringBuilderDotted = null;
            }
        }

        private static string GetDottedText(byte[] data, SettingsManager settingsManager)
        {
            var dataConverted = data.Select(ConvertNonPrintableAscii).ToArray();
            return settingsManager.SelectedPort.Settings.Encoding.GetString(dataConverted, 0, dataConverted.Length);
        }

        private static byte ConvertNonPrintableAscii(byte value) => IsPrintableAscii(value) ? value : (byte)'.';
        
        private static bool IsPrintableAscii(byte value) => value >= 33 && value <= 126;

        private string _consoleText;
        private string _consoleTextDotted;
        private StringBuilder _stringBuilder;
        private StringBuilder _stringBuilderDotted;
    }
}