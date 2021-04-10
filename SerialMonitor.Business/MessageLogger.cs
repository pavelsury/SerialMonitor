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

        public void PrintMessage(string message, EMessageType messageType)
        {
            ConsoleWriter?.WriteLine(message, messageType);
            PrintMessageToFile(message);
        }

        public void PrintInfoMessage(string message) => PrintMessage(message, EMessageType.Info);

        public void PrintWarningMessage(string message) => PrintMessage(message, EMessageType.Warning);

        public void PrintErrorMessage(string message) => PrintMessage(message, EMessageType.Error);

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