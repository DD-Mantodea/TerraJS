using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerraJS.JSEngine.Plugins;

namespace TerraJS.Contents.Utils
{
    public class AssemblyUtils
    {
        public static List<Type> GetExtensionTypes(Assembly[] assemblies)
        {
            List<Type> ret = [];

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => t.GetMethods().Any(m => m.GetCustomAttribute<ExtensionAttribute>() != null));

                ret.AddRange(types);
            }

            return ret;
        }

        public static List<Type> GetPluginTypes(Assembly[] assemblies)
        {
            List<Type> ret = [];

            var pluginType = typeof(TJSPlugin);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => t.IsSubclassOf(pluginType));

                ret.AddRange(types);
            }

            return ret;
        }
    }
}
