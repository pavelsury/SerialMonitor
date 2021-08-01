using System;
using System.Globalization;
using System.Windows.Data;
using SerialMonitor.Business.Enums;

namespace SerialMonitor.Win.Ui.Converters
{
    public class ConnectionStateToConnectTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connectionState = (EConnectionStatus)value;
            switch (connectionState)
            {
                case EConnectionStatus.Disconnected:
                case EConnectionStatus.ConnectingShort: return "Connect";
                case EConnectionStatus.ConnectingLong: return "Connecting...";
                case EConnectionStatus.DisconnectingGracefully:
                case EConnectionStatus.DisconnectingByFailure:
                case EConnectionStatus.Connected: return "Disconnect";
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}