using System;
using System.Text;
using Newtonsoft.Json;

namespace SerialMonitor.Business.Helpers
{
    public class EncodingJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Encoding)value).CodePage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.Value)
            {
                case int v: return GetEncoding(v);
                case long v: return GetEncoding((int)v);
                case string s: return int.TryParse(s, out var p) ? GetEncoding(p) : null;
                default: return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Encoding);
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