using System;
using System.IO;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public class MessageLogger : IMessageLogger
    {
        public MessageLogger(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public IConsoleWriter ConsoleWriter { private get; set; }

        public void PrintErrorMessage(string message)
        {
            ConsoleWriter?.WriteLine(message, EConsoleTextType.Error);
            PrintMessageToFile(message);
        }

        public void PrintWarningMessage(string message)
        {
            ConsoleWriter?.WriteLine(message, EConsoleTextType.Warning);
            PrintMessageToFile(message);
        }

        public void PrintSuccessMessage(string message)
        {
            ConsoleWriter?.WriteLine(message, EConsoleTextType.Success);
            PrintMessageToFile(message);
        }

        public void PrintProcessMessage(string message)
        {
            ConsoleWriter?.WriteLine(message, EConsoleTextType.Process);
            PrintMessageToFile(message);
        }

        private void PrintMessageToFile(string message)
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

        private readonly SettingsManager _settingsManager;
    }
}