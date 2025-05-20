using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    public class BoolArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public BoolArgument() : this("") { }

        public override bool FromString(string content, out object value)
        {
            value = null;

            if (bool.TryParse(content, out bool val))
            {
                value = val;

                return true;
            }

            return false;
        }

        public override string ToString() => IsOptional ? $"[<{Name} : bool>]" : $"<{Name} : bool>";

        public override bool InScope(object value) => value is string;
    }
}
