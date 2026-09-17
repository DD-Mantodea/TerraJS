using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class EnumArgument(string name, Type enumType, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public override bool FromString(string content, object last, out object value) => TryParse(content, last, out value, out _);

        public override bool TryParse(string content, object last, out object value, out string expected)
        {
            expected = ToString();

            value = null;

            foreach (var name in enumType.GetEnumNames())
            {
                if (!string.Equals(name, content, StringComparison.OrdinalIgnoreCase))
                    continue;

                value = Enum.Parse(enumType, name);

                return true;
            }

            return false;
        }

        public override Type InstanceType => typeof(decimal);

        public override List<string> GetCompletions(CommandInfo commandInfo) => DealStartWith([.. enumType.GetEnumNames()], commandInfo.CurrentParameter);

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            foreach (var name in enumType.GetEnumNames())
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

            foreach (var name in enumType.GetEnumNames())
            {
                if (name.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    return ArgumentState.Incomplete;
            }

            return ArgumentState.Invalid;
        }

        public override string ToString() => IsOptional ? $"[<{Name} : {enumType.Name}>]" : $"<{Name} : {enumType.Name}>";

        public override bool InScope(object value, object last) => enumType.GetEnumValues().Cast<object>().Contains(value);
    }
}
