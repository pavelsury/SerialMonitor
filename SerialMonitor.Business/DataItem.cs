using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public class DataItem
    {
        public DataItem(byte[] data, SettingsManager settingsManager)
        {
            MessageType = EMessageType.Data;
            Data = new List<byte>(data);
            _consoleText = settingsManager.SelectedPort.Settings.Encoding.GetString(data, 0, data.Length);
            HexData = BitConverter.ToString(data).Split('-').ToList();
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

        public EMessageType MessageType { get; }
        public bool IsStatusMessage => MessageType != EMessageType.Data;

        public List<byte> Data { get; }

        public List<string> HexData { get; }

        public string Text => _stringBuilder?.ToString() ?? _consoleText;

        public void Append(DataItem dataItem)
        {
            Data?.AddRange(dataItem.Data);
            HexData?.AddRange(dataItem.HexData);

            if (_stringBuilder == null)
            {
                _stringBuilder = new StringBuilder(_consoleText);
            }

            _stringBuilder.Append(dataItem.Text);
        }

        public void Freeze()
        {
            if (_stringBuilder != null)
            {
                _consoleText = _stringBuilder.ToString();
                _stringBuilder.Clear();
                _stringBuilder = null;
            }
        }

        private string _consoleText;
        private StringBuilder _stringBuilder;
    }
}