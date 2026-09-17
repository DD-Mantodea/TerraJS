using TerraJS.JSEngine.API.Commands.CommandArguments;
using TerraJS.JSEngine.API.Commands.CommandGUI;

namespace TerraJS.JSEngine.API.Commands.Completion
{
    public sealed class CompletionContext
    {
        public CommandInfo Info;

        public CommandArgument Argument;

        public string Prefix = string.Empty;

        public string RawPrefix = string.Empty;

        public string Suffix = string.Empty;

        public int ArgumentIndex;

        public bool InBracket;

        public bool InValue;

        public string AttributeName = string.Empty;

        public string AttributeOperator = string.Empty;

        public int ReplaceStart;

        public int ReplaceLength;

        public Suggestion Value(string insert, string display = null, string description = null, int matchLength = -1)
            => Suggestion.Create(insert, matchLength < 0 ? Prefix.Length : matchLength, ReplaceStart, ReplaceLength, SuggestionKind.Value, display, description);

        public Suggestion Hint(string display, string description = null)
            => Suggestion.Create(null, 0, ReplaceStart, ReplaceLength, SuggestionKind.Hint, display, description);

        public Suggestion Placeholder(string display, string description = null)
            => Suggestion.Create(null, 0, ReplaceStart, ReplaceLength, SuggestionKind.Placeholder, display, description);

        public CompletionContext With(CommandArgument argument)
        {
            return new CompletionContext
            {
                Info = Info,
                Argument = argument,
                Prefix = Prefix,
                RawPrefix = RawPrefix,
                Suffix = Suffix,
                ArgumentIndex = ArgumentIndex,
                InBracket = InBracket,
                InValue = InValue,
                AttributeName = AttributeName,
                AttributeOperator = AttributeOperator,
                ReplaceStart = ReplaceStart,
                ReplaceLength = ReplaceLength,
            };
        }
    }
}
