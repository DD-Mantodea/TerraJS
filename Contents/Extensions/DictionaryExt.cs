using System.Collections.Generic;

namespace TerraJS.Contents.Extensions
{
    public static class DictionaryExt
    {
        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.TryAdd(key, value))
                dict[key] = value;
        }
    }
}
