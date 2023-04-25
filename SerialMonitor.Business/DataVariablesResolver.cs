using SerialMonitor.Business.Data;
using SerialMonitor.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialMonitor.Business
{
    public class DataVariablesResolver
    {
        public DataVariablesResolver(IEndiannessProvider endiannessProvider)
        {
            _bitConverter = new EndianBitConverter(endiannessProvider);
        }

        public const string DataVariableName = "DATA";
        public static readonly string StartDelimiter = $@"{AppSettings.VariableStartDelimiter}{DataVariableName}";

        public byte[] Resolve(string dataVariable)
        {
            var tokens = dataVariable
                .ToLower()
                .RemoveWhitespaces()
                .Substring(StartDelimiter.Length)
                .TrimStart(AppSettings.DataAttributeDelimiter)
                .TrimEnd(AppSettings.VariableEndDelimiter)
                .Split(AppSettings.DataDelimiter);

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

        public string MakeVar(params string[] data)
        {
            return $"{StartDelimiter}{AppSettings.DataDelimiter}{string.Join(AppSettings.DataDelimiter.ToString(), data)}{AppSettings.VariableEndDelimiter}";
        }

        private byte[] GetBytes(string text, Attributes defaultAttributes)
        {
            var tokens = text.Split(AppSettings.DataAttributeDelimiter);
            if (tokens.Length > 2)
            {
                throw new Exception($"Invalid data value: {text}");
            }

            var valueText = tokens[0];
            var attributesText = tokens.Length == 2 ? tokens[1] : string.Empty;

            EBase prefixBase;
            (valueText, prefixBase) = GetPrefixBase(valueText);

            var valueAttributes = ParseAttributes(attributesText);

            if (valueAttributes.Base == EBase.None)
            {
                valueAttributes.Base = prefixBase;
            }
            
            if (prefixBase != EBase.None && valueAttributes.Base != prefixBase)
            {
                throw new Exception($"Invalid data attributes: {text}");
            }

            if (valueAttributes.Width == EWidth.None)
            {
                valueAttributes.Width = defaultAttributes.Width;
            }

            if (valueAttributes.Base == EBase.None)
            {
                valueAttributes.Base = defaultAttributes.Base;
            }

            return ParseValue(valueText, valueAttributes);
        }

        private static (string text, EBase prefixBase) GetPrefixBase(string text)
        {
            if (text.StartsWith(AppSettings.BinPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return (text.Remove(0, AppSettings.BinPrefix.Length), EBase.Binary);
            }

            if (text.StartsWith(AppSettings.OctPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return (text.Remove(0, AppSettings.OctPrefix.Length), EBase.Octal);
            }

            if (text.StartsWith(AppSettings.HexPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return (text.Remove(0, AppSettings.HexPrefix.Length), EBase.Hex);
            }

            return (text, EBase.None);
        }

        private byte[] ParseValue(string text, Attributes attributes)
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
                    case EWidth.W16: return _bitConverter.GetBytes(Convert.ToUInt16(text, attributes.BaseNumber));
                    case EWidth.W32: return _bitConverter.GetBytes(Convert.ToUInt32(text, attributes.BaseNumber));
                    case EWidth.W64: return _bitConverter.GetBytes(Convert.ToUInt64(text, attributes.BaseNumber));
                    default: throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception($"Invalid data value: {text}");
            }
        }

        private byte[] ParseDecimalValue(string text, EWidth widthAttribute)
        {
            switch (widthAttribute)
            {
                case EWidth.None: return ParseDecimalValueAutoWidth(text);
                case EWidth.W8: return new[] { sbyte.TryParse(text, out var value8) ? (byte)value8 : byte.Parse(text) };
                case EWidth.W16: return short.TryParse(text, out var value16) ? _bitConverter.GetBytes(value16) : _bitConverter.GetBytes(ushort.Parse(text));
                case EWidth.W32: return int.TryParse(text, out var value32) ? _bitConverter.GetBytes(value32) : _bitConverter.GetBytes(uint.Parse(text));
                case EWidth.W64: return long.TryParse(text, out var value64) ? _bitConverter.GetBytes(value64) : _bitConverter.GetBytes(ulong.Parse(text));
                default: throw new Exception();
            }
        }

        private byte[] ParseDecimalValueAutoWidth(string text)
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

        private byte[] ParsePositiveValueMinWidth(ulong value)
        {
            if (value <= byte.MaxValue)
            {
                return new[] { (byte)value };
            }

            if (value <= ushort.MaxValue)
            {
                return _bitConverter.GetBytes((ushort)value);
            }

            if (value <= uint.MaxValue)
            {
                return _bitConverter.GetBytes((uint)value);
            }

            return _bitConverter.GetBytes(value);
        }

        private byte[] ParseNegativeValueMinWidth(long value)
        {
            if (value >= sbyte.MinValue)
            {
                return new[] { (byte)value };
            }

            if (value >= short.MinValue)
            {
                return _bitConverter.GetBytes((short)value);
            }

            if (value >= int.MinValue)
            {
                return _bitConverter.GetBytes((int)value);
            }

            return _bitConverter.GetBytes(value);
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
                var index = attributesText.IndexOf(width.ToString(), StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                {
                    attributes.Width = widthEnum;
                    attributesText = attributesText.Remove(index, width.ToString().Length);
                    break;
                }
            }

            foreach (var (name, baseEnum) in supportedBases)
            {
                var index = attributesText.IndexOf(name.ToString(), StringComparison.OrdinalIgnoreCase);
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

        private EndianBitConverter _bitConverter;
    }
}
