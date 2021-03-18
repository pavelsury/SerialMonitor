using System;

namespace SerialMonitor.Business
{
    public interface IUsbNotification
    {
        event EventHandler<bool> DeviceChanged;
    }
}