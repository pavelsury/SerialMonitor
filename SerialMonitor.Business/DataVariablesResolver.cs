using SerialMonitor.Business.Data;
using SerialMonitor.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialMonitor.Business
{
    public static class DataVariablesResolver
    {
        public static readonly string StartDelimiter = $@"{AppSettings.VariableStartDelimiter}DATA";

        public static byte[] Resolve(string dataVariable)
        {
            var tokens = dataVariable
                .ToLower()
                .RemoveWhitespaces()
                .Substring(StartDelimiter.Length)
                .TrimStart(AppSettings.DataAttributeDelimiter)
                .TrimEnd(AppSettings.VariableEndDelimiter)
                .Split(',');

            if (tokens.Length <= 1)
            {
                throw new Exception($"No data in data variable: {dataVariable}");
            }

            var defaultAttributes = ParseAttributes(tokens[0]);
            if (defaultAttributes.Base == EBase.None)
            {
                defaultAttributes.Base = EBase.Decimal;
            }

            var bytes = tokens.Skip(1).SelectMany(t => GetBytes(t, defaultAttributes)).ToArray();
            return bytes;
        }

        private static byte[] GetBytes(string text, Attributes defaultAttributes)
        {
            var tokens = text.Split(AppSettings.DataAttributeDelimiter);
            if (tokens.Length > 2)
            {
                throw new Exception($"Invalid data value: {text}");
            }

            var valueText = tokens[0];
            var attributesText = tokens.Length == 2 ? tokens[1] : string.Empty;

            var isHexPrefix = false;
            if (valueText.StartsWith(AppSettings.HexPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                isHexPrefix = true;
                valueText = text.Remove(0, 2);
            }

            var valueAttributes = ParseAttributes(attributesText);

            if (isHexPrefix)
            {
                if (valueAttributes.Base == EBase.None)
                {
                    valueAttributes.Base = EBase.Hex;
                }

                if (valueAttributes.Base != EBase.Hex)
                {
                    throw new Exception($"Invalid data attributes: {text}");
                }
            }

            if (valueAttributes.Width == EWidth.None)
            {
                valueAttributes.Width = defaultAttributes.Width;
            }

            if (valueAttributes.Base == EBase.None)
            {
                valueAttributes.Base = defaultAttributes.Base;
            }

            var bytes = ParseValue(valueText, valueAttributes);
            return bytes;
        }

        private static byte[] ParseValue(string text, Attributes attributes)
        {
            try
            {
                if (attributes.Base == EBase.Decimal)
                {
                    return ParseDecimalValue(text, attributes.Width);
                }

                switch (attributes.Width)
                {
                    case EWidth.None: return ParsePositiveValueMinWidth(Convert.ToUInt64(text, attributes.BaseNumber));
                    case EWidth.W8: return new[] { Convert.ToByte(text, attributes.BaseNumber) };
                    case EWidth.W16: return BitConverter.GetBytes(Convert.ToUInt16(text, attributes.BaseNumber));
                    case EWidth.W32: return BitConverter.GetBytes(Convert.ToUInt32(text, attributes.BaseNumber));
                    case EWidth.W64: return BitConverter.GetBytes(Convert.ToUInt64(text, attributes.BaseNumber));
                    default: throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception($"Invalid data value: {text}");
            }
        }

        private static byte[] ParseDecimalValue(string text, EWidth widthAttribute)
        {
            switch (widthAttribute)
            {
                case EWidth.None: return ParseDecimalValueAutoWidth(text);
                case EWidth.W8: return new[] { sbyte.TryParse(text, out var value8) ? (byte)value8 : byte.Parse(text) };
                case EWidth.W16: return short.TryParse(text, out var value16) ? BitConverter.GetBytes(value16) : BitConverter.GetBytes(ushort.Parse(text));
                case EWidth.W32: return int.TryParse(text, out var value32) ? BitConverter.GetBytes(value32) : BitConverter.GetBytes(uint.Parse(text));
                case EWidth.W64: return long.TryParse(text, out var value64) ? BitConverter.GetBytes(value64) : BitConverter.GetBytes(ulong.Parse(text));
                default: throw new Exception();
            }
        }

        private static byte[] ParseDecimalValueAutoWidth(string text)
        {
            if (!long.TryParse(text, out var value))
            {
                return ParsePositiveValueMinWidth(ulong.Parse(text));
            }

            if (value >= 0)
            {
                return ParsePositiveValueMinWidth((ulong)value);
            }

            return ParseNegativeValueMinWidth(value);
        }

        private static byte[] ParsePositiveValueMinWidth(ulong value)
        {
            if (value <= byte.MaxValue)
            {
                return new[] { (byte)value };
            }

            if (value <= ushort.MaxValue)
            {
                return BitConverter.GetBytes((ushort)value);
            }

            if (value <= uint.MaxValue)
            {
                return BitConverter.GetBytes((uint)value);
            }

            return BitConverter.GetBytes(value);
        }

        private static byte[] ParseNegativeValueMinWidth(long value)
        {
            if (value >= sbyte.MinValue)
            {
                return new[] { (byte)value };
            }

            if (value >= short.MinValue)
            {
                return BitConverter.GetBytes((short)value);
            }

            if (value >= int.MinValue)
            {
                return BitConverter.GetBytes((int)value);
            }

            return BitConverter.GetBytes(value);
        }

        private static Attributes ParseAttributes(string attributesText)
        {
            var origAttributesText = attributesText;
            var attributes = new Attributes();
            
            var supportedWidths = new List<(int, EWidth)>
            {
                (8, EWidth.W8),
                (16, EWidth.W16),
                (32, EWidth.W32),
                (64, EWidth.W64)
            };

            var supportedBases = new List<(char, EBase)> 
            {
                ('b', EBase.Binary), 
                ('o', EBase.Octal),
                ('d', EBase.Decimal), 
                ('x', EBase.Hex)
            };

            foreach (var (width, widthEnum) in supportedWidths)
            {
                var index = attributesText.IndexOf(width.ToString(), StringComparison.InvariantCultureIgnoreCase);
                if (index != -1)
                {
                    attributes.Width = widthEnum;
                    attributesText = attributesText.Remove(index, width.ToString().Length);
                    break;
                }
            }

            foreach (var (name, baseEnum) in supportedBases)
            {
                var index = attributesText.IndexOf(name.ToString(), StringComparison.InvariantCultureIgnoreCase);
                if (index != -1)
                {
                    attributes.Base = baseEnum;
                    attributesText = attributesText.Remove(index, 1);
                    break;
                }
            }

            if (attributesText.Length > 0)
            {
                throw new Exception($"Unknown data attributes: {origAttributesText}");
            }

            return attributes;
        }

        private enum EWidth
        {
            None,
            W8,
            W16,
            W32,
            W64
        }
            
        private enum EBase
        {
            None = 0,
            Binary = 2,
            Octal = 8,
            Decimal = 10,
            Hex = 16
        }

        private class Attributes
        {
            public EWidth Width { get; set; }
            public EBase Base { get; set; }
            public int BaseNumber => (int)Base;
        }
    }
}
