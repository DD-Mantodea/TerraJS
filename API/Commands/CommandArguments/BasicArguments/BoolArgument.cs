using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Attributes;
using TerraJS.Utils;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    [BindToEngine]
    public class BoolArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public BoolArgument() : this("", false) { }

        public static BoolArgument New(string name, dynamic options = default)
            => new(name, OptionUtils.GetOption<bool>(options, "isOptional", false));

        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (bool.TryParse(content, out bool val))
            {
                value = val;

                return true;
            }

            return false;
        }

        public override Type InstanceType => typeof(bool);

        public override string ToString() => IsOptional ? $"[<{Name} : bool>]" : $"<{Name} : bool>";

        public override bool InScope(object value, object last) => value is string;
    }
}
