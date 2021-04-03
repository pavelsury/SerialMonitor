using System;
using System.Text;
using Newtonsoft.Json;

namespace SerialMonitor.Business.Helpers
{
    public class EncodingJsonConverter : JsonConverter<Encoding>
    {
        public override void WriteJson(JsonWriter writer, Encoding value, JsonSerializer serializer)
        {
            writer.WriteValue(value.CodePage);
        }

        public override Encoding ReadJson(JsonReader reader, Type objectType, Encoding existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.Value)
            {
                case int v: return GetEncoding(v);
                case string s: return int.TryParse(s, out var p) ? GetEncoding(p) : null;
                default: return null;
            }
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