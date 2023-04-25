using System;

namespace SerialMonitor.Business.Helpers
{
    internal class EndianBitConverter
    {

        public EndianBitConverter(bool endianness)
        {
            _isLittleEndianFunc = () => endianness;
        }


        public EndianBitConverter(IEndiannessProvider endiannessProvider)
        {
            _isLittleEndianFunc = () => endiannessProvider.IsLittleEndian;
        }

        public byte[] GetBytes(short value) => ReverseBytesConditionally(BitConverter.GetBytes(value));
        public byte[] GetBytes(int value) => ReverseBytesConditionally(BitConverter.GetBytes(value));
        public byte[] GetBytes(long value) => ReverseBytesConditionally(BitConverter.GetBytes(value));
        public byte[] GetBytes(ushort value) => ReverseBytesConditionally(BitConverter.GetBytes(value));
        public byte[] GetBytes(uint value) => ReverseBytesConditionally(BitConverter.GetBytes(value));
        public byte[] GetBytes(ulong value) => ReverseBytesConditionally(BitConverter.GetBytes(value));

        private byte[] ReverseBytesConditionally(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian != _isLittleEndianFunc())
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        
        private readonly Func<bool> _isLittleEndianFunc;
    }
}
