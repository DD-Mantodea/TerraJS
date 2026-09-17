using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments
{
    public abstract class CommandArgument(string name, bool isOptional = false)
    {
        public CommandArgument() : this("") { }

        public string Name { get; set; } = name;

        public bool IsOptional { get; set; } = isOptional;

        public abstract bool FromString(string content, object last, out object value);

        public virtual bool TryParse(string content, object last, out object value, out string expected)
        {
            expected = ToString();

            return FromString(content, last, out value);
        }

        public abstract override string ToString();

        public abstract List<string> GetCompletions(CommandInfo commandInfo);

        public virtual IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            foreach (var value in GetCompletions(context.Info))
            {
                var insert = StripMarkup(value);

                if (!insert.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(insert, insert, null, context.Prefix.Length);
            }
        }

        public virtual ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            return ArgumentState.Valid;
        }

        public override int GetHashCode() => Name.GetHashCode();

        public virtual bool InScope(object value, object last) => true;

        public virtual bool SameType(CommandArgument arg) => GetType().FullName == arg.GetType().FullName;

        public virtual bool SameValue(CommandArgument arg) => SameType(arg);

        public static bool operator ==(CommandArgument c1, CommandArgument c2) => c1.Name == c2.Name;

        public static bool operator !=(CommandArgument c1, CommandArgument c2) => !(c1 == c2);

        public static List<string> DealStartWith(List<string> values, string match)
        {
            return [.. values.Where(t => t.StartsWith(match)).Select(t => (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + t[match.Length..])];
        }

        private static readonly Regex MarkupPattern = new(@"\[c/[0-9A-Fa-f]{6}:((?:[^\[\]]|\[[^\[\]]*\])*)\]");

        public static string StripMarkup(string text) => string.IsNullOrEmpty(text) ? string.Empty : MarkupPattern.Replace(text, "$1");

        public virtual Type InstanceType => typeof(object);

        public override bool Equals(object obj)
        {
            if (obj is null or not CommandArgument) return false;

            return this == (CommandArgument)obj;
        }
    }
}
