using System;
using System.Collections.Generic;

namespace SerialMonitor.Business.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }
    }
}