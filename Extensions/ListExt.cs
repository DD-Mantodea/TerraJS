using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Extensions
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

        public static void TryAddRange<T>(this List<T> list, T[] items)
        {
            foreach (var item in items)
                list.TryAdd(item);
        }
    }
}
