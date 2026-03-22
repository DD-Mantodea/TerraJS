using System.Collections.Generic;

namespace TerraJS.Contents.Extensions
{
    public static class ListExt
    {
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
    }
}
