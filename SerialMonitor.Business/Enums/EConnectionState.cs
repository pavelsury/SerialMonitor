namespace SerialMonitor.Business.Enums
{
    public enum EConnectionState
    {
        Disconnected,
        DisconnectingGracefully,
        DisconnectingByFailure,
        ConnectingShort,
        ConnectingLong,
        Connected
    }
}