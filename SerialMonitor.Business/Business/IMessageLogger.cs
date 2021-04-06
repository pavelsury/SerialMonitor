namespace SerialMonitor.Business
{
    public interface IMessageLogger
    {
        void PrintErrorMessage(string message);

        void PrintWarningMessage(string message);

        void PrintSuccessMessage(string message);

        void PrintProcessMessage(string message);
    }
}