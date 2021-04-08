namespace SerialMonitor.Business
{
    public interface IMessageLogger
    {
        void PrintInfoMessage(string message);
        void PrintWarningMessage(string message);
        void PrintErrorMessage(string message);
    }
}