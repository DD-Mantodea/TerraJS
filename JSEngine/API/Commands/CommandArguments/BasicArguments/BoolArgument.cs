using System;
using System.Collections.Generic;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class BoolArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public BoolArgument() : this("", false) { }

        public static BoolArgument New(string name, dynamic options = default)
            => new(name, OptionUtils.GetOption<bool>(options, "isOptional", false));

        public override bool FromString(string content, object last, out object value) => TryParse(content, last, out value, out _);

        public override bool TryParse(string content, object last, out object value, out string expected)
        {
            expected = ToString();

            value = null;

            foreach (var text in Values)
            {
                if (!string.Equals(text, content, StringComparison.OrdinalIgnoreCase))
                    continue;

                value = bool.Parse(text);

                return true;
            }

            return false;
        }

        public override Type InstanceType => typeof(bool);

        public override string ToString() => IsOptional ? $"[<{Name} : bool>]" : $"<{Name} : bool>";

        public override List<string> GetCompletions(CommandInfo commandInfo) => DealStartWith(["true", "false"], commandInfo.CurrentParameter);

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            foreach (var value in Values)
            {
                if (!value.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(value, value, null, context.Prefix.Length);
            }
        }

        private static readonly string[] Values = ["true", "false"];

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (TryParse(token, null, out _, out _))
                return ArgumentState.Valid;

            foreach (var value in Values)
            {
                if (value.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    return ArgumentState.Incomplete;
            }

            return ArgumentState.Invalid;
        }

        public override bool InScope(object value, object last) => value is bool;
    }
}
