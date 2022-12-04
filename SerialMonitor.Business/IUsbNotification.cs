using System;

namespace SerialMonitor.Business
{
    public interface IUsbNotification : IDisposable
    {
        event EventHandler<string> PortRemoved;
        event EventHandler<string> PortArrived;
    }
}