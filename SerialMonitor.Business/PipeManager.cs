using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialMonitor.Business
{
    public class PipeManager
    {
        public PipeManager(
            PortManager portManager,
            SettingsManager settingsManager,
            ConsoleManager consoleManager,
            IMainThreadRunner mainThreadRunner)
        {
            _portManager = portManager;
            _settingsManager = settingsManager;
            _consoleManager = consoleManager;
            _mainThreadRunner = mainThreadRunner;
            _settingsManager.PropertyChanged += OnSettingsManagerChanged;
        }

        private void OnSettingsManagerChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SettingsManager.SelectedPort):
                    if (_settingsManager.PipeEnabled)
                    {
                        UpdatePipe();
                    }
                    break;

                case nameof(SettingsManager.PipeEnabled):
                    if (_settingsManager.PipeEnabled)
                    {
                        UpdatePipe();
                    }
                    else
                    {
                        ClosePipe();
                    }
                    break;
            }
        }

        private void UpdatePipe()
        {
            if (SelectedPort == null || SelectedPort.Name == _portName)
            {
                return;
            }

            ClosePipe();
            
            _cts = new CancellationTokenSource();
            
            _portName = SelectedPort.Name;

            _consoleManager.PrintInfoMessage($"Pipe opened: {PipeName}");
            
            _task = Task.Run(async () =>
            {
                NamedPipeServerStream pipeServer = null;
                _cts.Token.Register(() => pipeServer?.Dispose());

                try
                {
                    var buffer = new byte[100];
                    var messageBuffer = new byte[100];

                    while (true)
                    {
                        pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                        await pipeServer.WaitForConnectionAsync(_cts.Token);

                        var totalCount = 0;

                        while (true)
                        {
                            _cts.Token.ThrowIfCancellationRequested();
                            var count = await pipeServer.ReadAsync(buffer, 0, buffer.Length);
                            if (count == 0)
                            {
                                pipeServer.Dispose();
                                break;
                            }

                            Buffer.BlockCopy(buffer, 0, messageBuffer, totalCount, count);
                            totalCount += count;

                            if (pipeServer.IsMessageComplete)
                            {
                                HandlePipeCommand(messageBuffer, totalCount);
                                totalCount = 0;
                            }
                        }

                        _cts.Token.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception)
                { }
                finally
                {
                    pipeServer?.Dispose();
                }
            });
        }

        private void ClosePipe()
        {
            try
            {
                _cts?.Cancel();
            }
            catch (AggregateException)
            {
            }
            
            _task?.Wait();
            _task = null;
            _cts?.Dispose();
            _cts = null;

            if (_portName != null)
            {
                _consoleManager.PrintInfoMessage($"Pipe closed: {PipeName}");
            }
            _portName = null;
        }

        private void HandlePipeCommand(byte[] buffer, int count)
        {
            var command = Encoding.UTF8.GetString(buffer, 0, count);

            switch (command)
            {
                case "connect":
                    _mainThreadRunner.Run(() =>
                    {
                        PrintPipeCommand(command);
                        _portManager.Connect();
                    });
                    break;
                
                case "disconnect":
                    _mainThreadRunner.Run(() =>
                    {
                        PrintPipeCommand(command);
                        _portManager.Disconnect();
                    });
                    break;

                default:
                    _mainThreadRunner.Run(() => _consoleManager.PrintWarningMessage($"Unknown pipe command: {command}"));
                    break;
            }
        }

        private void PrintPipeCommand(string command) => _consoleManager.PrintInfoMessage($"Pipe command: {command}");

        private string PipeName => $"SerialMonitorPipe{_portName}";

        private PortInfo SelectedPort => _settingsManager.SelectedPort;

        private readonly PortManager _portManager;
        private readonly SettingsManager _settingsManager;
        private readonly ConsoleManager _consoleManager;
        private readonly IMainThreadRunner _mainThreadRunner;
        private string _portName;
        private Task _task;
        private CancellationTokenSource _cts;
    }
}
