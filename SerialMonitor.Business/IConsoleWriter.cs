using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public interface IConsoleWriter
    {
        void Write(string text, EMessageType messageType);
        void Clear();
    }
}