using System;

namespace SerialMonitor.Business
{
    public interface IUsbNotification : IDisposable
    {
        event EventHandler<bool> DeviceChanged;
    }
}