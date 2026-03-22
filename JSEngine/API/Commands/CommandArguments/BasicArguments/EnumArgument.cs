using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.JSEngine.API.Commands.CommandGUI;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class EnumArgument(string name, Type enumType, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (enumType.GetEnumNames().Contains(content))
            {
                value = Enum.Parse(enumType, content);

                return true;
            }

            return false;
        }

        public override Type InstanceType => typeof(decimal);

        public override List<string> GetCompletions(CommandInfo commandInfo) => DealStartWith([.. enumType.GetEnumNames()], commandInfo.CurrentParameter);

        public override string ToString() => IsOptional ? $"[<{Name} : {enumType.Name}>]" : $"<{Name} : {enumType.Name}>";

        public override bool InScope(object value, object last) => enumType.GetEnumValues().Cast<object>().Contains(value);
    }
}
