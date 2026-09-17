using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class ConstantArgument(string name, string content, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public readonly string _content = content;   

        public static ConstantArgument New(string name, dynamic options = default)
            => new(name, OptionUtils.GetOption<bool>(options, "isOptional", false));


        public override bool FromString(string content, object last, out object value) => TryParse(content, last, out value, out _);

        public override bool TryParse(string content, object last, out object value, out string expected)
        {
            expected = ToString();

            value = null;

            if (!string.Equals(content, _content, StringComparison.OrdinalIgnoreCase))
                return false;

            value = _content;

            return true;
        }

        public override bool SameValue(CommandArgument arg)
        {
            var cons = arg as ConstantArgument;

            return cons._content == _content;
        }

        public override Type InstanceType => typeof(string);

        public override string ToString() => IsOptional ? $"[<{Name} = {_content}>]" : $"<{Name} = {_content}>";

        public override List<string> GetCompletions(CommandInfo commandInfo) => DealStartWith([_content], commandInfo.CurrentParameter);

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            if (!_content.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                yield break;

            yield return context.Value(_content, _content, null, context.Prefix.Length);
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (TryParse(token, null, out _, out _))
                return ArgumentState.Valid;

            return _content.StartsWith(token, StringComparison.OrdinalIgnoreCase) ? ArgumentState.Incomplete : ArgumentState.Invalid;
        }

        public override bool InScope(object value, object last) => value is string str && str == _content;

        private static Regex Pattern => new("\"(.*)\"");
    }
}
