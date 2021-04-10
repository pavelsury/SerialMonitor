using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public interface IMessageLogger
    {
        void PrintMessage(string message, EMessageType messageType);
        void PrintInfoMessage(string message);
        void PrintWarningMessage(string message);
        void PrintErrorMessage(string message);
    }
}