using System.Collections.Generic;

namespace SerialMonitor.Business.Helpers
{
    public static class IDictionaryExtensions
    {
        public static TValue GetValueOrDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var value) ? value : default(TValue);
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            if (dict.TryGetValue(key, out var val))
            {
                return val;
            }

            val = new TValue();
            dict.Add(key, val);
            return val;
        }
    }
}