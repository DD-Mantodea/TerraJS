using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace TerraJS.Contents.Extensions
{
    public static class ListExt
    {
        private static readonly Random _random = new();

        public static bool TryAdd<T>(this List<T> list, T item)
        {
            if (list.Contains(item)) 
                return false;

            list.Add(item);

            return true;
        }

        public static void TryAddRange<T>(this List<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
                list.TryAdd(item);
        }

        public static T Random<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;

            return list[_random.Next(list.Count)];
        }
    }
}
