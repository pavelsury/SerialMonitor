using System;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public class DataManager
    {
        public DataManager(
            SettingsManager settingsManager,
            ConsoleManager consoleManager,
            IConnectionStatusProvider connectionStatusProvider,
            IMainThreadRunner mainThreadRunner)
        {
            _settingsManager = settingsManager;
            _consoleManager = consoleManager;
            _connectionStatusProvider = connectionStatusProvider;
            _mainThreadRunner = mainThreadRunner;
        }

        public void Clean()
        {
            _isSpareCr = false;
            lock (_dataItemLock)
            {
                _dataItem = null;
            }
        }

        public void ProcessReceivedData(byte[] buffer, int bytesCount)
        {
            byte[] data;

            if (SelectedPort.Settings.ReceivingNewline == EReceivingNewline.Crlf)
            {
                var isLastCr = buffer[bytesCount - 1] == '\r';

                var dataOffset = _isSpareCr ? 1 : 0;
                var dataLength = _isSpareCr ? bytesCount + 1 : bytesCount;
                var countToCopy = isLastCr ? bytesCount - 1 : bytesCount;

                if (isLastCr)
                {
                    dataLength -= 1;
                }

                if (dataLength <= 0)
                {
                    _isSpareCr = isLastCr;
                    return;
                }

                data = new byte[dataLength];
                Buffer.BlockCopy(buffer, 0, data, dataOffset, countToCopy);

                if (_isSpareCr)
                {
                    data[0] = (byte)'\r';
                }

                _isSpareCr = isLastCr;
            }
            else
            {
                data = new byte[bytesCount];
                Buffer.BlockCopy(buffer, 0, data, 0, bytesCount);
            }

            var dataItem = new DataItem(data, _settingsManager);

            lock (_dataItemLock)
            {
                if (_dataItem == null)
                {
                    _dataItem = dataItem;
                    _mainThreadRunner.Run(ProcessReceivedData);
                }
                else
                {
                    _dataItem.Append(dataItem);
                }
            }
        }

        private PortInfo SelectedPort => _settingsManager.SelectedPort;

        private EConnectionStatus ConnectionStatus => _connectionStatusProvider.ConnectionStatus;

        private void ProcessReceivedData()
        {
            DataItem dataItem = null;
            
            lock (_dataItemLock)
            {
                if (ConnectionStatus == EConnectionStatus.Connected)
                {
                    dataItem = _dataItem;
                }
                _dataItem = null;
            }

            if (dataItem != null)
            {
                _consoleManager.Write(dataItem);
            }
        }

        private readonly SettingsManager _settingsManager;
        private readonly ConsoleManager _consoleManager;
        private readonly IConnectionStatusProvider _connectionStatusProvider;
        private readonly IMainThreadRunner _mainThreadRunner;
        private DataItem _dataItem;
        private readonly object _dataItemLock = new object();
        private bool _isSpareCr;
    }
}