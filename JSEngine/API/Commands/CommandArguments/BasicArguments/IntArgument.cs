using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class IntArgument(string name, int minValue = int.MinValue, int maxValue = int.MaxValue, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public IntArgument() : this("", isOptional: false) { }

        public static IntArgument New(string name, dynamic options = default)
            => new(
                name, 
                OptionUtils.GetOption<int>(options, "minValue", int.MinValue),
                OptionUtils.GetOption<int>(options, "maxValue", int.MaxValue),
                OptionUtils.GetOption<bool>(options, "isOptional", false)
            );


        private readonly int _minVal = minValue;

        private readonly int _maxVal = maxValue >= minValue ? maxValue : minValue;

        public override bool FromString(string content, object last, out object value) => TryParse(content, last, out value, out _);

        public override bool TryParse(string content, object last, out object value, out string expected)
        {
            expected = ToString();

            value = null;

            if (!long.TryParse(content, out var result))
                return false;

            if (result < _minVal || result > _maxVal)
                return false;

            value = (int)result;

            return true;
        }

        public override string ToString()
        {
            var scope = "";

            if (_minVal != int.MinValue)
            {
                if (_maxVal != int.MaxValue)
                    scope = $"({_minVal}, {_maxVal})";
                else scope = $"({_minVal}, INTMAX)";
            }
            else if (_maxVal != int.MaxValue)
                scope = $"(INTMIN, {_maxVal})";

            var ret = $"<{Name} : int{scope}>";

            return IsOptional ? $"[{ret}]" : ret;
        }

        public override List<string> GetCompletions(CommandInfo commandInfo) => [];

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            var prefix = context.Prefix;

            if (_maxVal - (long)_minVal <= 20 && _minVal != int.MinValue)
            {
                for (long value = _minVal; value <= _maxVal; value++)
                {
                    var text = value.ToString();

                    if (!text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    yield return context.Value(text, text, null, prefix.Length);
                }

                yield break;
            }

            yield return context.Hint(ToString());
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (TryParse(token, null, out _, out _))
                return ArgumentState.Valid;

            return token is "-" or "+" ? ArgumentState.Incomplete : ArgumentState.Invalid;
        }

        public override Type InstanceType => typeof(int);

        public override bool InScope(object value, object last) => value is int v && v >= _minVal && v <= _maxVal;

    }
}
