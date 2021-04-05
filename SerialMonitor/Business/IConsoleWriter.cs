using SerialMonitor.Business.Enums;

namespace SerialMonitor.Business
{
    public interface IConsoleWriter
    {
        void WriteText(string text);
        void WriteLine(string text, EConsoleTextType consoleTextType);
    }
}