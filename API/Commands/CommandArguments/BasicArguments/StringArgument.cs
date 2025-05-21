using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    public class StringArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public StringArgument() : this("") { }

        public override bool FromString(string content, out object value)
        {
            value = null;

            if (!Pattern.IsMatch(content))
                return false;

            value = Pattern.Match(content).Groups[1].Value;

            return true;
        }

        public override Type InstanceType => typeof(string);

        public override string ToString() => IsOptional ? $"[<{Name} : string>]" : $"<{Name} : string>";

        public override bool InScope(object value) => value is string;

        private static Regex Pattern => new("\"(.*)\"");
    }
}
