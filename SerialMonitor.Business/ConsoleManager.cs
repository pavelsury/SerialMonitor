using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public class ConsoleManager
    {
        public ConsoleManager(SettingsManager settingsManager, FileOutputManager fileOutputManager)
        {
            _settingsManager = settingsManager;
            _fileOutputManager = fileOutputManager;
        }

        public void Initialize(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
            _settingsManager.PropertyChanged += OnSettingsManagerChanged;
        }

        public void ClearAll()
        {
            _dataItems.Clear();
            ClearConsole();
            _fileOutputManager.EnsureNewline();
        }

        public void Write(DataItem dataItem)
        {
            InsertDataItem(dataItem);
            PrintToConsole(dataItem, true);
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
        }

        public void PrintInfoMessage(string message) => PrintMessage(message, EMessageType.Info);
        public void PrintCommand(string message) => PrintMessage(message, EMessageType.Command);

        public void PrintWarningMessage(string message) => PrintMessage(message, EMessageType.Warning);

        public void PrintErrorMessage(string message) => PrintMessage(message, EMessageType.Error);

        private void PrintToConsole(DataItem dataItem, bool writeToFile)
        {
            if (dataItem.MessageType == EMessageType.Data)
            {
                switch (_settingsManager.ViewMode)
                {
                    case EViewMode.Text:
                        PrintToConsole(dataItem.Text, EMessageType.Data, writeToFile);
                        return;

                    case EViewMode.Hex:
                        PrintHexToConsole(dataItem.HexData, writeToFile);
                        return;
                    
                    case EViewMode.HexColumns:
                        PrintFixedHexToConsole(dataItem.HexData, writeToFile);
                        return;
                    
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            if (dataItem.MessageType == EMessageType.Command)
            {
                if (!_settingsManager.AppSettings.WriteCommandToConsole)
                {
                    EnsureNewline(writeToFile);
                    return;
                }
            }
            else
            {
                if (!_settingsManager.AppSettings.WriteMessageToConsole)
                {
                    EnsureNewline(writeToFile);
                    return;
                }
            }

            var text = _isLastNewline ? dataItem.Text : $"{Environment.NewLine}{dataItem.Text}";
            PrintToConsole(text, dataItem.MessageType, writeToFile);
        }

        private void PrintHexToConsole(List<string> hexData, bool writeToFile)
        {
            _hexStringBuilder.Clear();

            foreach (var hex in hexData)
            {
                AppendHex(_isLastNewline, hex);

                _isLastNewline = hex == "0A";
                if (_isLastNewline)
                {
                    _hexStringBuilder.AppendLine();
                }
            }

            PrintToConsole(_hexStringBuilder.ToString(), EMessageType.Data, writeToFile);
        }

        private void PrintFixedHexToConsole(List<string> hexData, bool writeToFile)
        {
            _hexStringBuilder.Clear();
            if (_isLastNewline)
            {
                _currentHexCount = 0;
            }

            foreach (var hex in hexData)
            {
                AppendHex(_currentHexCount == 0, hex);

                _currentHexCount++;

                if (_currentHexCount == _settingsManager.HexFixedColumns)
                {
                    _hexStringBuilder.AppendLine();
                    _currentHexCount = 0;
                }
            }

            PrintToConsole(_hexStringBuilder.ToString(), EMessageType.Data, writeToFile);
        }

        private void AppendHex(bool isFirst, string hex)
        {
            if (!isFirst)
            {
                _hexStringBuilder.Append(_settingsManager.HexSeparator);
            }

            if (_settingsManager.HexPrefixEnabled)
            {
                _hexStringBuilder.Append("0x");
            }

            _hexStringBuilder.Append(hex);
        }

        private void PrintToConsole(string text, EMessageType messageType, bool writeToFile)
        {
            _isLastNewline = text.EndsWith("\n");
            _consoleWriter.Write(text, messageType);
            if (writeToFile)
            {
                _fileOutputManager.Write(text);
            }
        }

        private void OnSettingsManagerChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SettingsManager.ViewMode):
                case nameof(SettingsManager.WriteMessageToConsole):
                case nameof(SettingsManager.WriteCommandToConsole):
                    ReprintAll();
                    return;

                case nameof(SettingsManager.HexPrefixEnabled):
                case nameof(SettingsManager.HexSeparator):
                    if (_settingsManager.ViewMode == EViewMode.Hex ||
                        _settingsManager.ViewMode == EViewMode.HexColumns)
                    {
                        ReprintAll();
                    }
                    return;

                case nameof(SettingsManager.HexFixedColumns):
                    if (_settingsManager.ViewMode == EViewMode.HexColumns)
                    {
                        ReprintAll();
                    }
                    return;
            }
        }

        private void ReprintAll()
        {
            ClearConsole();

            _dataItems.ForEach(d => PrintToConsole(d, false));
            _fileOutputManager.EnsureNewline();
        }

        public void EnsureNewline(bool writeToFile)
        {
            if (!_isLastNewline)
            {
                PrintToConsole(Environment.NewLine, EMessageType.Info, writeToFile);
            }
        }

        private void ClearConsole()
        {
            _consoleWriter.Clear();
            _isLastNewline = true;
            _currentHexCount = 0;
        }

        private readonly SettingsManager _settingsManager;
        private readonly FileOutputManager _fileOutputManager;
        private IConsoleWriter _consoleWriter;
        private readonly List<DataItem> _dataItems = new List<DataItem>();
        private int _currentHexCount;
        private readonly StringBuilder _hexStringBuilder = new StringBuilder();
        private bool _isLastNewline = true;
    }
}