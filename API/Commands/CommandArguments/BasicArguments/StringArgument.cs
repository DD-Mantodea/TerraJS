using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerraJS.Attributes;
using TerraJS.Utils;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    [BindToEngine]
    public class StringArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public StringArgument() : this("", false) { }

        public static StringArgument New(string name, dynamic options = default)
            => new(name, OptionUtils.GetOption<bool>(options, "isOptional", false));


        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (!Pattern.IsMatch(content))
                return false;

            value = Pattern.Match(content).Groups[1].Value;

            return true;
        }

        public override Type InstanceType => typeof(string);

        public override string ToString() => IsOptional ? $"[<{Name} : string>]" : $"<{Name} : string>";

        public override bool InScope(object value, object last) => value is string;

        private static Regex Pattern => new("\"(.*)\"");
    }
}
