using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SerialMonitor.Business
{
    public class FileOutputManager
    {
        public FileOutputManager(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void Write(string text)
        {
            
            _isLastNewline = text.EndsWith("\n");

            lock (_dataLock)
            {
                _data.Add(text);
                if (!_isWritingInProgress)
                {
                    _isWritingInProgress = true;
                    Task.Run(ProcessData);
                }
            }
        }

        public void WriteNewlineConditionally()
        {
            if (!_isLastNewline)
            {
                Write(Environment.NewLine);
            }
        }

        private void ProcessData()
        {
            while (true)
            {
                List<string> data;
                lock (_dataLock)
                {
                    if (!_data.Any())
                    {
                        _isWritingInProgress = false;
                        return;
                    }

                    data = _data.ToList();
                    _data.Clear();
                }

                try
                {
                    foreach (var text in data)
                    {
                        File.AppendAllText(_settingsManager.SelectedPort.Settings.OutputFilename, text);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private readonly SettingsManager _settingsManager;
        private bool _isWritingInProgress;
        private readonly List<string> _data = new List<string>();
        private readonly object _dataLock = new object();
        private bool _isLastNewline = true;
    }
}