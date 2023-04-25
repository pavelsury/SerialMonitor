using System;

namespace SerialMonitor.Business.Helpers
{
    public static class ByteArrayExtensions
    {
        public static string[] ToHexStringArray(this byte[] bytes)
        {
            return bytes.Length > 0 ? BitConverter.ToString(bytes).Split('-') : Array.Empty<string>();
        }
    }
}
