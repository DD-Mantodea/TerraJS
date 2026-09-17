using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.MultipleArguments
{
    public class ListArgument<T>(string name, int minLength = 0, int maxLength = int.MaxValue, bool isOptional = false) : CommandArgument(name, isOptional) where T : CommandArgument, new()
    {
        private T _argumentInstance = new();

        private int _minLength = minLength >= 0 ? minLength : 0;

        private int _maxLength = maxLength >= minLength ? maxLength : minLength;

        public override bool FromString(string content, object last, out object value) => TryParse(content, last, out value, out _);

        public override bool TryParse(string content, object last, out object value, out string expected)
        {
            expected = ToString();

            value = null;

            if (!Pattern.IsMatch(content))
                return false;

            var list = new List<object>();

            var args = Pattern.Match(content).Groups[1].Value.Replace(" ", "").SplitListElements();

            object lastArg = null;

            foreach (var arg in args)
            {
                if (!_argumentInstance.TryParse(arg, lastArg, out var val, out _))
                    return false;

                list.Add(val);

                lastArg = val;
            }

            if (list.Count < _minLength || list.Count > _maxLength)
                return false;

            value = list;

            return true;
        }

        public override string ToString()
        {
            var scope = "";

            if (_maxLength != int.MaxValue)
                scope = $"length({_minLength}, {_maxLength})";
            else scope = $"length({_minLength}, INTMAX)";

            var content = $"<{Name} : list<{typeof(T).Name}> {scope}>";

            return IsOptional ? $"[{content}]" : content;
        }

        public override bool InScope(object value, object last)
        {
            return value is List<object> list && list.Count >= _minLength && list.Count <= _maxLength;
        }

        public override List<string> GetCompletions(CommandInfo commandInfo) => [];

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            var info = context.Info;

            if (info is null || !info.InBracket)
            {
                yield return context.Hint(ToString());

                yield break;
            }

            var sub = new CompletionContext
            {
                Info = info,
                Argument = _argumentInstance,
                Prefix = context.Prefix,
                RawPrefix = context.RawPrefix,
                Suffix = context.Suffix,
                ArgumentIndex = 0,
                InBracket = false,
                InValue = false,
                AttributeName = string.Empty,
                AttributeOperator = string.Empty,
                ReplaceStart = info.SegmentStart,
                ReplaceLength = Math.Max(0, info.SegmentEnd - info.SegmentStart),
            };

            foreach (var suggestion in _argumentInstance.Complete(sub))
                yield return suggestion;
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (TryParse(token, null, out _, out _))
                return ArgumentState.Valid;

            if (token.IndexOf('[') < 0)
                return ArgumentState.Invalid;

            return token.EndsWith("]", StringComparison.Ordinal) ? ArgumentState.Invalid : ArgumentState.Incomplete;
        }

        public override Type InstanceType => typeof(List<>);

        private static Regex Pattern => new("\\[(.*)\\]");
    }
}
