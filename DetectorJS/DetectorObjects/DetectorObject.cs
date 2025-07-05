using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.Contents.Utils;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public abstract class DetectorObject
    {
        public abstract string Serialize();

        public static string Type2ImportName(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
                return Type2PrimitiveName(type);

            var name = type.Name;

            if (type.IsGenericType)
                name = name.Split("`")[0] + type.GetGenericArguments().Length;

            if (type.IsNested && !type.IsGenericTypeParameter)
                name = $"{Type2ImportName(type.DeclaringType)}${name}";

            if (BindingUtils.Values.Exists(turple => turple.Item1 == name))
                name = "$" + name;

            return name;
        }

        public static string Type2ParameterName(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
                return Type2PrimitiveName(type);

            var name = type.Name;

            if (type.IsGenericType)
                name = name.Split("`")[0] + type.GetGenericArguments().Length;

            if (type.IsNested && !type.IsGenericTypeParameter)
                name = $"{Type2ImportName(type.DeclaringType)}${name}";

            if (type.IsArray)
            {
                var element = type.GetElementType();

                name = Type2ParameterName(element);

                name += "s";
            }

            return name;
        }

        public static string Type2ClassName(Type type, string[] nameData = null, bool asParameter = false)
        {
            if (type.IsPrimitive || type == typeof(string))
                return Type2PrimitiveName(type);

            var name = type.Name;

            if (type.IsGenericType)
            {
                name = name.Split("`")[0];

                if (asParameter)
                {
                    if (name == "Func")
                    {
                        var genericArguments = type.GetGenericArguments().ToList();

                        genericArguments.RemoveAt(genericArguments.Count - 1);

                        var index = 0;

                        var args = string.Join(", ", [.. genericArguments.Select(type => {
                        return $"{(nameData == null ? Type2ParameterName(type) : nameData[index++])}: {Type2ClassName(type)}";
                    })]);

                        name = $"({args}) => {Type2ClassName(type.GetGenericArguments().Last())}";
                    }
                    else if (name == "Action")
                    {
                        var genericArguments = type.GetGenericArguments().ToList();

                        var index = 0;

                        var args = string.Join(", ", [.. genericArguments.Select(type => {
                        return $"{(nameData == null ? Type2ParameterName(type) : nameData[index++])}: {Type2ClassName(type)}";
                    })]);

                        name = $"({args}) => void";
                    }
                }
                else
                {
                    var args = string.Join(", ", [.. type.GetGenericArguments().ToList().Select(type => Type2ClassName(type))]);

                    name = $"{name}{type.GetGenericArguments().Length}<{args}>";
                }
            }

            if (type == typeof(Action) && asParameter)
                name = $"() => void";

            if (type.IsNested && !type.IsGenericTypeParameter)
                name = $"{Type2ImportName(type.DeclaringType)}${name}";

            if (type.IsArray)
            {
                var element = type.GetElementType();

                name = Type2ClassName(element) + string.Concat(Enumerable.Repeat("[]", type.GetArrayRank()));
            }

            return name;
        }

        public static string Type2PrimitiveName(Type type)
        {
            List<Type> numberList = [
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double)
            ];

            if (type == typeof(bool))
                return "boolean";

            if (numberList.Contains(type))
                return "number";

            if (type == typeof(string) || type == typeof(char))
                return "string";

            return type.Name;
        }

        public static string SpecialNameCheck(string name)
        {
            if (name == "function")
                return "$function";

            if (name == "continue")
                return "$continue";

            if (name == "default")
                return "$default";

            if (name == "finally")
                return "$finally";

            if (name == "break")
                return "$break";

            if (name == "enum")
                return "$enum";

            return name;
        }
    }
}
