using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Utils
{
    static class Extenstions
    {
        public static void AddUnique<K,V>(this IDictionary<K,V> dict, K key, V value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
        }

        public static long RoundUpTo(this long value, long boundry)
        {
            long num = value / boundry;
            if (value % boundry != 0)
                num++;
            return num * boundry;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var cur in enumerable)
                collection.Add(cur);
        }

        public static void InsertRange<T>(this IList<T> collection, int index, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
                collection.Insert(index++, item);
        }

        public static IList<T> Prepend<T>(this IList<T> list, T item)
        {
            return new List<T> { item }.Concat(list).ToList();
        }

        public static void AddMany<T>(this ICollection<T> collection, IEnumerable<IEnumerable<T>> many)
        {
            foreach (var item in many)
                collection.AddRange(item);
        }

        public static void AddMany<T>(this ICollection<T> collection, params IEnumerable<T>[] many)
        {
            collection.AddMany((IEnumerable<IEnumerable<T>>)many);
        }

        public static bool IsCombo(this Enum e, Enum other)
        {
            if (e.GetType().IsEquivalentTo(other.GetType()))
                throw new Exception("Bad enum types");

            ulong v1 = Convert.ToUInt64(e), v2 = Convert.ToUInt64(other);
            return (v1 & v2) == v1;
        }
    }
}
