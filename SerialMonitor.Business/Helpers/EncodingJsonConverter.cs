using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerialMonitor.Business.Helpers
{
    public class EncodingJsonConverter : JsonConverter<Encoding>
    {
        public override Encoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert != typeof(Encoding))
            {
                return null;
            }

            switch (reader.TokenType)
            {
                case JsonTokenType.String: return int.TryParse(reader.GetString(), out var v) ? GetEncoding(v) : null;
                case JsonTokenType.Number: return reader.TryGetInt32(out v) ? GetEncoding(v) : null;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.CodePage);
        }

        private static Encoding GetEncoding(int codePage)
        {
            try
            {
                return Encoding.GetEncoding(codePage);
            }
            catch (Exception e) when (e is ArgumentOutOfRangeException || e is ArgumentException || e is NotSupportedException)
            {
                return null;
            }
        }
    }
}