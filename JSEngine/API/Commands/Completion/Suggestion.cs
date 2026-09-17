namespace TerraJS.JSEngine.API.Commands.Completion
{
    public enum SuggestionKind
    {
        Command,
        Value,
        Placeholder,
        Hint,
    }

    public sealed class Suggestion
    {
        public string Insert;

        public string Display;

        public string Description;

        public SuggestionKind Kind = SuggestionKind.Value;

        public int MatchLength;

        public int ReplaceStart;

        public int ReplaceLength;

        public bool Insertable => (Kind is SuggestionKind.Command or SuggestionKind.Value) && !string.IsNullOrEmpty(Insert);

        public static Suggestion Create(string insert, int matchLength, int replaceStart, int replaceLength, SuggestionKind kind = SuggestionKind.Value, string display = null, string description = null)
        {
            return new Suggestion
            {
                Insert = insert,
                Display = display ?? insert ?? string.Empty,
                Description = description,
                Kind = kind,
                MatchLength = matchLength,
                ReplaceStart = replaceStart,
                ReplaceLength = replaceLength,
            };
        }
    }
}
