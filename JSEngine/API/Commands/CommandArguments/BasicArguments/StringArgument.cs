using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class StringArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public StringArgument() : this("", false) { }

        public static StringArgument New(string name, dynamic options = default)
            => new(name, OptionUtils.GetOption<bool>(options, "isOptional", false));


        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (!Pattern.IsMatch($"\"{content}\""))
                return false;

            string matchedValue = Pattern.Match($"\"{content}\"").Groups[1].Value;

            value = UnescapeString(matchedValue);

            return true;
        }

        public override Type InstanceType => typeof(string);

        public override string ToString() => IsOptional ? $"[<{Name} : string>]" : $"<{Name} : string>";

        public override List<string> GetCompletions(CommandInfo commandInfo) => [];

        public override IEnumerable<Suggestion> Complete(CompletionContext context) => [context.Hint("\"<text>\"")];

        public override bool InScope(object value, object last) => value is string;

        private static Regex Pattern => new("\"(.*)\"");

        private static string UnescapeString(string escapedString)
        {
            var result = new StringBuilder();
            bool escapeNext = false;

            foreach (char c in escapedString)
            {
                if (escapeNext)
                {
                    switch (c)
                    {
                        case 'n': result.Append('\n'); break;
                        case 'r': result.Append('\r'); break;
                        case 't': result.Append('\t'); break;
                        case '0': result.Append('\0'); break;
                        case '\\': result.Append('\\'); break;
                        case '"': result.Append('"'); break;
                        case '\'': result.Append('\''); break;
                        default: result.Append('\\').Append(c); break;
                    }
                    escapeNext = false;
                }
                else if (c == '\\')
                    escapeNext = true;
                else
                    result.Append(c);
            }

            if (escapeNext)
                result.Append('\\');

            return result.ToString();
        }
    }
}
