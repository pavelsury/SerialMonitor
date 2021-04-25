using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public class ConsoleManager
    {
        public ConsoleManager(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void Initialize(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
            _settingsManager.PropertyChanged += OnSettingsChanged;
            UpdateHexCount();
        }

        public void ClearAll()
        {
            _dataItems.Clear();
            ClearConsole();
        }

        public void Write(DataItem dataItem)
        {
            InsertDataItem(dataItem);
            PrintToConsole(dataItem);
        }

        private void InsertDataItem(DataItem dataItem)
        {
            var lastDataItem = _dataItems.LastOrDefault();
            if (lastDataItem == null)
            {
                _dataItems.Add(dataItem);
                return;
            }

            if (lastDataItem.MessageType == dataItem.MessageType)
            {
                lastDataItem.Append(dataItem);
                return;
            }
            
            lastDataItem.Freeze();
            _dataItems.Add(dataItem);
        }

        public void PrintMessage(string message, EMessageType messageType)
        {
            if (!message.EndsWith("\n"))
            {
                message += Environment.NewLine;
            }
            Write(new DataItem(message, messageType));
            //WriteMessageToFile(message);
        }

        public void PrintInfoMessage(string message) => PrintMessage(message, EMessageType.Info);

        public void PrintWarningMessage(string message) => PrintMessage(message, EMessageType.Warning);

        public void PrintErrorMessage(string message) => PrintMessage(message, EMessageType.Error);

        private void PrintToConsole(DataItem dataItem)
        {
            if (dataItem.IsStatusMessage)
            {
                var text = _isLastNewline ? dataItem.Text : $"{Environment.NewLine}{dataItem.Text}";
                PrintToConsole(text, dataItem.MessageType);
                return;
            }

            if (_isHexOutput)
            {
                PrintHexToConsole(dataItem.HexData);
            }
            else
            {
                PrintToConsole(dataItem.Text, EMessageType.Data);
            }
        }

        private void PrintHexToConsole(List<string> hexData)
        {
            if (_hexCount == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _hexStringBuilder.Clear();
            if (_isLastNewline)
            {
                _currentHexCount = 0;
            }

            foreach (var hex in hexData)
            {
                _hexStringBuilder.Append(_currentHexCount == 0 ? hex : $" {hex}");
                _currentHexCount++;

                if (_currentHexCount == _hexCount)
                {
                    _hexStringBuilder.AppendLine();
                    _currentHexCount = 0;
                }
            }

            PrintToConsole(_hexStringBuilder.ToString(), EMessageType.Data);
        }

        private void PrintToConsole(string text, EMessageType messageType)
        {
            _isLastNewline = text.EndsWith("\n");
            _consoleWriter?.Write(text, messageType);
        }

        private void WriteMessageToFile(string message)
        {
            if (_settingsManager.SelectedPort.Settings.OutputToFileEnabled)
            {
                var file = _settingsManager.SelectedPort.Settings.OutputFilename;

                if (!string.IsNullOrEmpty(file))
                {
                    File.AppendAllText(file, $@"{message}{Environment.NewLine}");
                }
            }
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsManager.ViewMode))
            {
                ReprintAll();
            }
        }

        private void ReprintAll()
        {
            if (_consoleWriter == null)
            {
                return;
            }

            UpdateHexCount();
            ClearConsole();
            _dataItems.ForEach(PrintToConsole);
        }

        private void UpdateHexCount()
        {
            switch (_settingsManager.ViewMode)
            {
                case EViewMode.Text:
                    _hexCount = 0;
                    _isHexOutput = false;
                    return;
                case EViewMode.Hex1: _hexCount = 1; break;
                case EViewMode.Hex2: _hexCount = 2; break;
                case EViewMode.Hex4: _hexCount = 4; break;
                case EViewMode.Hex8: _hexCount = 8; break;
                case EViewMode.Hex16: _hexCount = 16; break;
                case EViewMode.Hex32: _hexCount = 32; break;
                default: throw new ArgumentOutOfRangeException();
            }

            _isHexOutput = true;
        }

        private void ClearConsole()
        {
            _consoleWriter.Clear();
            _isLastNewline = true;
            _currentHexCount = 0;
        }

        private readonly SettingsManager _settingsManager;
        private IConsoleWriter _consoleWriter;
        private readonly List<DataItem> _dataItems = new List<DataItem>();
        private bool _isHexOutput;
        private int _hexCount;
        private int _currentHexCount;
        private readonly StringBuilder _hexStringBuilder = new StringBuilder();
        private bool _isLastNewline = true;
    }
}