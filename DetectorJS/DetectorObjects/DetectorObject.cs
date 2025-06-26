using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Attributes;
using TerraJS.Extensions;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.DetectorJS.DetectorObjects
{
    [Indetectable]
    public abstract class DetectorObject
    {
        public abstract string Serialize();

        public string Type2ImportName(Type type)
        {
            var name = type.Name;

            if (type.IsGenericType)
                name = name.Split("`")[0] + type.GetGenericArguments().Length;

            if (type.IsNested && !type.IsGenericTypeParameter)
                name = $"{Type2ImportName(type.DeclaringType)}${name}";

            return name;
        }

        public string Type2ClassName(Type type)
        {
            var name = type.Name;

            if (type.IsGenericType)
            {
                name = name.Split("`")[0];

                var args = string.Join(", ", [.. type.GetGenericArguments().ToList().Select(Type2ClassName)]);

                name = $"{name}{type.GetGenericArguments().Length}<{args}>";
            }

            if (type.IsFixedBufferType())
                name = name.Replace("<", "__").Replace(">", "__");

            if (type.IsNested && !type.IsGenericTypeParameter)
                name = $"{Type2ImportName(type.DeclaringType)}${name}";

            if (type.IsArray)
            {
                var element = type.GetElementType();

                name = Type2ClassName(element) + string.Concat(Enumerable.Repeat("[]", type.GetArrayRank()));
            }

            return name;
        }
    }
}
