using System;

namespace SerialMonitor.Business.Helpers
{
    public static class ByteArrayExtensions
    {
        public static byte[] EndianReverse(this byte[] bytes, bool useLittleEndian)
        {
            if (BitConverter.IsLittleEndian != useLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static string[] ToHexStringArray(this byte[] bytes)
        {
            return bytes.Length > 0 ? BitConverter.ToString(bytes).Split('-') : new string[0];
        }
    }
}
