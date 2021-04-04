using System;
using System.IO;

namespace SerialMonitor.Business
{
    public class MessageLogger : IMessageLogger
    {
        public MessageLogger(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public IMessageLogger ConsoleMessageLogger { private get; set; }

        public void PrintErrorMessage(string message)
        {
            ConsoleMessageLogger?.PrintErrorMessage(message);
            PrintMessageToFile(message);
        }

        public void PrintWarningMessage(string message)
        {
            ConsoleMessageLogger?.PrintWarningMessage(message);
            PrintMessageToFile(message);
        }

        public void PrintSuccessMessage(string message)
        {
            ConsoleMessageLogger?.PrintSuccessMessage(message);
            PrintMessageToFile(message);
        }

        public void PrintProcessMessage(string message)
        {
            ConsoleMessageLogger?.PrintProcessMessage(message);
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