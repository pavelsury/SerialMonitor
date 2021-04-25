using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public interface IConnectionStatusProvider
    {
        EConnectionStatus ConnectionStatus { get; }
    }
}