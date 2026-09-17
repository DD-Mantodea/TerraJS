using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;
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

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            foreach (var name in PrefixID.Search.Names)
            {
                if (!name.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(name, name, null, context.Prefix.Length);
            }
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (TryParse(token, null, out _, out _))
                return ArgumentState.Valid;

            var prefixMatched = false;

            foreach (var name in PrefixID.Search.Names)
            {
                if (name.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    prefixMatched = true;
            }

            return prefixMatched ? ArgumentState.Incomplete : ArgumentState.Invalid;
        }

        public override string ToString() => IsOptional ? $"[<{Name} : Prefix>]" : $"<{Name} : Prefix>";
    }
}
