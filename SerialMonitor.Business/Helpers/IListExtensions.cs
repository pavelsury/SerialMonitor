using System;
using System.Collections.Generic;

namespace SerialMonitor.Business.Helpers
{
    public static class IListExtensions
    {
        public static void AddSorted<T>(this IList<T> collection, T newItem, IComparer<T> comparer)
        {
            for (var index = 0; index < collection.Count; ++index)
            {
                var item = collection[index];
                if (comparer.Compare(newItem, item) <= 0)
                {
                    collection.Insert(index, newItem);
                    return;
                }
            }
            collection.Add(newItem);
        }

        public static void AddSorted<T>(this IList<T> collection, T newItem) where T : IComparable<T>
        {
            AddSorted(collection, newItem, Comparer<T>.Create((x, y) => x.CompareTo(y)));
        }
    }
}
