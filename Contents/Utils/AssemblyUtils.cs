using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                foreach (var type in SafeGetTypes(assembly))
                {
                    if (HasExtensionMethod(type))
                        ret.Add(type);
                }
            }

            return ret;
        }

        public static List<Type> GetPluginTypes(Assembly[] assemblies)
        {
            List<Type> ret = [];

            var pluginType = typeof(TJSPlugin);

            foreach (var assembly in assemblies)
            {
                foreach (var type in SafeGetTypes(assembly))
                {
                    if (!IsPlugin(type, pluginType))
                        continue;

                    ret.Add(type);
                }
            }

            return ret;
        }

        private static bool HasExtensionMethod(Type type)
        {
            try
            {
                return type.GetMethods().Any(method => method.GetCustomAttribute<ExtensionAttribute>() is not null);
            }
            catch (Exception e)
            {
                Warn($"{type.FullName} skipped: {e.Message}");

                return false;
            }
        }

        private static bool IsPlugin(Type type, Type pluginType)
        {
            try
            {
                if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
                    return false;

                return type.IsSubclassOf(pluginType);
            }
            catch (Exception e)
            {
                Warn($"{type.FullName} skipped: {e.Message}");

                return false;
            }
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                var types = (e.Types ?? []).Where(type => type is not null).ToList();

                Warn($"{assembly.GetName().Name}: {e.LoaderExceptions?.Length ?? 0} type(s) failed to load, {types.Count} skipped.");

                return types;
            }
            catch (Exception e)
            {
                Warn($"{assembly.GetName().Name} skipped: {e.Message}");

                return [];
            }
        }

        private static void Warn(string message) => TerraJS.Instance?.Logger.Warn($"[Plugin] {message}");
    }
}
