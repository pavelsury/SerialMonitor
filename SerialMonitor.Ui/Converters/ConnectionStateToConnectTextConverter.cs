using System;
using System.Globalization;
using System.Windows.Data;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Ui.Converters
{
    public class ConnectionStateToConnectTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connectionState = (EConnectionState)value;
            switch (connectionState)
            {
                case EConnectionState.Disconnected:
                case EConnectionState.ConnectingShort: return "Connect";
                case EConnectionState.ConnectingLong: return "Connecting...";
                case EConnectionState.DisconnectingGracefully:
                case EConnectionState.DisconnectingByFailure:
                case EConnectionState.Connected: return "Disconnect";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}