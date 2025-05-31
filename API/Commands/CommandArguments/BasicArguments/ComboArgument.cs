using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    public class ComboArgument(string name, List<string> enableValues, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        internal List<string> _enableValues = enableValues;

        public override bool FromString(string content, out object value)
        {
            value = null;

            if (!_enableValues.Contains(content))
                return false;

            value = content;

            return true;
        }

        public override bool FromStringWithoutClamp(string content, out object value)
        {
            value = content;

            return true;
        }

        public override string ToString() => IsOptional ? $"[<{Name}({string.Join("/", _enableValues.Take(2))}..) : string>]" : $"<{Name}({string.Join(", ", _enableValues.Take(2))}..) : string>";

        public override bool InScope(object value) => value is string str && _enableValues.Contains(str);

        public override Type InstanceType => typeof(string);
    }
}
