using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TerraJS.API;

namespace TerraJS.JSEngine.API.DotNets.Enums
{
    public class EnumRegistry : IRegistry<Enum>
    {
        internal EnumBuilder _builder;

        internal static Dictionary<string, Type> _tjsEnums = [];

        public bool IsEmpty = false;

        public static EnumRegistry Empty => new() { IsEmpty = true };

        public EnumRegistry() { }

        public EnumRegistry(EnumBuilder builder)
        {
            _builder = builder;
        }

        public EnumRegistry DefineLiteral(string name, int value)
        {
            if (IsEmpty) return this;

            _builder.DefineLiteral(name, value);

            return this;
        }

        public void Register()
        {
            if (IsEmpty)
                return;

            var enumType = _builder.CreateType();

            _tjsEnums.Add(_builder.Name, enumType);
        }
    }
}
