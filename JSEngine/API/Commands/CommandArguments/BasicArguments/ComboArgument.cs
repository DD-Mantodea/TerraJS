using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.Contents.Utils;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    public class ComboArgument(string name, string[] enableValues, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        internal List<string> _enableValues = [..enableValues];

        public static ComboArgument New(string name, string[] enableValues, dynamic options = default)
            => new(name, enableValues, OptionUtils.GetOption<bool>(options, "isOptional", false));

        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (!_enableValues.Contains(content))
                return false;

            value = content;

            return true;
        }

        public override bool FromStringWithoutClamp(string content, object last, out object value)
        {
            value = content;

            return true;
        }

        public override string ToString() => IsOptional ? $"[<{Name}({string.Join("/", _enableValues.Take(2))}..) : string>]" : $"<{Name}({string.Join(", ", _enableValues.Take(2))}..) : string>";

        public override bool InScope(object value, object last) => value is string str && _enableValues.Contains(str);

        public override Type InstanceType => typeof(string);
    }
}
