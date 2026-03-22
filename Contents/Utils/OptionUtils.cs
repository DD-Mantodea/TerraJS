using System.Collections.Generic;
using System.Dynamic;

namespace TerraJS.Contents.Utils
{
    public class OptionUtils
    {
        public static T GetOption<T>(ExpandoObject obj, string name, T defaultValue)
        {
            if (obj == null)
                return defaultValue;

            var options = (IDictionary<string, object>)obj;

            return options.TryGetValue(name, out object value) ? (T)value : defaultValue;
        }
    }
}
