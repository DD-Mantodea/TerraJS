using System;
using System.Collections.Generic;
using Jint.Runtime.Interop;
using TerraJS.JSEngine;

namespace TerraJS.Contents.Utils
{
    public class BindingUtils
    {
        public static List<(string Name, object Value)> Values = [];

        public static void BindInstance(string name, object instance)
        {
            if (!TryAdd(name, instance))
                return;

            Apply(name, instance);
        }

        public static void BindStaticOrEnumOrConst(string name, Type type)
        {
            if (!TryAdd(name, type))
                return;

            if (TJSEngine.Engine is { } engine)
                engine.SetValue(name, TypeReference.CreateTypeReference(engine, type));
        }

        public static void BindInnerMethod(string name, Delegate @delegate)
        {
            if (!TryAdd(name, @delegate))
                return;

            Apply(name, @delegate);
        }

        public static void Flush()
        {
            if (TJSEngine.Engine is not { } engine)
                return;

            foreach (var (name, value) in Values)
                engine.SetValue(name, value is Type type ? TypeReference.CreateTypeReference(engine, type) : value);
        }

        public static void Clear() => Values.Clear();

        private static void Apply(string name, object value)
        {
            if (TJSEngine.Engine is { } engine)
                engine.SetValue(name, value);
        }

        private static bool TryAdd(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (Values.Exists(binding => binding.Name == name))
            {
                TerraJS.Instance?.Logger.Warn($"[Jint] binding \"{name}\" is bound more than once, the later one wins.");

                Values.RemoveAll(binding => binding.Name == name);
            }

            Values.Add((name, value));

            return true;
        }
    }
}
