namespace SerialMonitor.Business.Enums
{
    public enum EConnectionStatus
    {
        Disconnected,
        DisconnectingGracefully,
        DisconnectingByFailure,
        ConnectingShort,
        ConnectingLong,
        Connected
    }
}