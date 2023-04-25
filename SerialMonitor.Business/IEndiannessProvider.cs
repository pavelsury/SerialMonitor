namespace SerialMonitor.Business
{
    public interface IEndiannessProvider
    {
        bool IsLittleEndian { get; }
    }
}
