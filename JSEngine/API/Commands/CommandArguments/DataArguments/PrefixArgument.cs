using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria.ID;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.DataArguments
{
    public class PrefixArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public override bool FromString(string content, object last, out object value)
        {
            var ret = PrefixID.Search.TryGetId(content, out var id);

            value = id;

            return ret;
        }

        public override List<string> GetCompletions(CommandInfo commandInfo) => DealStartWith([.. PrefixID.Search.Names], commandInfo.CurrentParameter);

        public override string ToString() => IsOptional ? $"[<{Name} : Prefix>]" : $"<{Name} : Prefix>";
    }
}
