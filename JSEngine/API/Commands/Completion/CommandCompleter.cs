using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraJS.JSEngine.API.Commands.CommandArguments;
using TerraJS.JSEngine.API.Commands.CommandGUI;

namespace TerraJS.JSEngine.API.Commands.Completion
{
    public sealed class CompletionResult
    {
        public List<Suggestion> Items = [];

        public string Header = string.Empty;

        public string Footer = string.Empty;

        public string Error = string.Empty;
    }

    public static class CommandCompleter
    {
        public const int MaxSuggestions = 100;

        static CommandCompleter()
        {
            SetDefaultTranslation("Commands.Completion.More", "{0} more, keep typing to narrow down", "还有 {0} 条，继续输入以缩小范围");
        }

        public static CompletionResult Complete(CommandInfo info)
        {
            var result = new CompletionResult();

            if (info is null || info.State == InputState.Empty)
                return result;

            var commands = CommandSource.ChatCommands;

            if (info.State == InputState.Command)
                CompleteCommandName(info, commands, result);
            else if (HasExactCommand(info, commands))
                CompleteArguments(info, commands, result);
            else
                CompleteCommandWordError(info, commands, result);

            result.Items.Sort(Compare);

            if (result.Items.Count > MaxSuggestions)
            {
                var hidden = result.Items.Count - MaxSuggestions;

                result.Items.RemoveRange(MaxSuggestions, hidden);

                result.Footer = More(hidden);
            }

            return result;
        }

        private static string More(int hidden)
        {
            var key = "Commands.Completion.More";

            var translation = TJSEngine.GlobalAPI?.Translation;

            var text = default(string);

            if (translation is not null)
            {
                text = translation.GetTranslation(key, Language.ActiveCulture);

                if (string.IsNullOrEmpty(text) || text == key)
                    translation.DefaultLocalizedTexts.TryGetValue(key, out text);
            }

            if (string.IsNullOrEmpty(text) || text == key)
                return hidden + " more";

            try
            {
                return string.Format(text, hidden);
            }
            catch (FormatException)
            {
                return text;
            }
        }

        private static void SetDefaultTranslation(string key, string english, string chinese)
        {
            var translation = TJSEngine.GlobalAPI?.Translation;

            if (translation is null)
                return;

            translation.SetDefaultTranslation(GameCulture.DefaultCulture, key, english);

            translation.SetDefaultTranslation(GameCulture.FromLegacyId(7), key, chinese);
        }

        private static bool HasExactCommand(CommandInfo info, List<ModCommand> commands)
        {
            foreach (var command in commands)
            {
                if (string.Equals(command.Command, info.Command, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static void CompleteCommandWordError(CommandInfo info, List<ModCommand> commands, CompletionResult result)
        {
            var incomplete = false;

            foreach (var command in commands)
            {
                if (command.Command.StartsWith(info.Command, StringComparison.OrdinalIgnoreCase))
                {
                    incomplete = true;

                    break;
                }
            }

            result.Error = incomplete
                ? (Language.ActiveCulture.LegacyId == 7 ? $"命令名不完整: {info.Command}" : $"Incomplete command name: {info.Command}")
                : (Language.ActiveCulture.LegacyId == 7 ? "未知命令" : "Unknown command");
        }

        private static void CompleteCommandName(CommandInfo info, List<ModCommand> commands, CompletionResult result)
        {
            var prefix = info.MatchPrefix;

            var names = new List<string>();

            var usages = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var command in commands)
            {
                var name = command.Command;

                if (string.IsNullOrEmpty(name))
                    continue;

                if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!usages.TryGetValue(name, out var list))
                {
                    list = [];

                    usages[name] = list;

                    names.Add(name);
                }

                if (command is TJSCommand tjs && tjs.TryGetArgumentsText([], out var text))
                {
                    var usage = CommandArgument.StripMarkup(text).Trim();

                    if (usage.Length > 0 && !list.Contains(usage))
                        list.Add(usage);
                }

                if (!string.IsNullOrEmpty(command.Description) && !descriptions.ContainsKey(name))
                    descriptions[name] = command.Description;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var name in names)
            {
                var usage = string.Join(" | ", usages[name]);

                var display = usage.Length == 0 ? name : name + " " + usage;

                if (!descriptions.TryGetValue(name, out var description) || string.IsNullOrEmpty(description))
                    description = Language.ActiveCulture.LegacyId == 7 ? "无描述" : "No description";

                Add(result.Items, seen, Suggestion.Create(name, prefix.Length, info.ReplaceStart, info.ReplaceLength, SuggestionKind.Command, display, description));
            }

            if (result.Items.Count == 0 && prefix.Length > 0)
                result.Error = Language.ActiveCulture.LegacyId == 7 ? "未知命令" : "Unknown command";
        }

        private static void CompleteArguments(CommandInfo info, List<ModCommand> commands, CompletionResult result)
        {
            var token = info.CurrentToken;

            if (!token.Closed)
            {
                result.Error = Language.ActiveCulture.LegacyId == 7 ? "引号未闭合" : "Unclosed quote";

                return;
            }

            if (info.InBracket && !token.Raw.EndsWith("]", StringComparison.Ordinal))
            {
                result.Error = Language.ActiveCulture.LegacyId == 7 ? "方括号未闭合" : "Unclosed bracket";

                return;
            }

            var typed = new List<string>();

            for (var i = 1; i < info.CurrentTokenIndex && i < info.Tokens.Count; i++)
                typed.Add(info.Tokens[i].Text);

            var prefix = info.MatchPrefix;

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var placeholders = new List<Suggestion>();

            var matched = false;

            var accepted = false;

            var expected = string.Empty;

            var usageParts = new List<string>();

            foreach (var command in commands)
            {
                if (command is not TJSCommand tjs)
                    continue;

                if (!tjs.Command.StartsWith(info.Command, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!CommandAPI.CommandArgumentGroups.TryGetValue(tjs.GetType().FullName, out var group))
                    continue;

                matched = true;

                var usage = group.RenderUsage();

                if (usage.Length > 0 && !usageParts.Contains(usage))
                    usageParts.Add(usage);

                var template = new CompletionContext
                {
                    Info = info,
                    Prefix = prefix,
                    RawPrefix = info.Prefix,
                    Suffix = info.Suffix,
                    ArgumentIndex = info.ParameterIndex,
                    InBracket = info.InBracket,
                    InValue = info.CursorInValue,
                    AttributeName = info.AttributeName,
                    AttributeOperator = info.AttributeOperator,
                    ReplaceStart = info.ReplaceStart,
                    ReplaceLength = info.ReplaceLength,
                };

                var paths = CommandTree.Walk(group.Root, typed, template, out var overrun);

                var candidates = new List<CommandNode>();

                foreach (var path in paths)
                    path.Node.Consumers(candidates);

                if (overrun && candidates.Count == 0)
                    continue;

                var viable = false;

                foreach (var candidate in candidates)
                {
                    var before = result.Items.Count;

                    foreach (var suggestion in candidate.Suggest(template))
                        Add(result.Items, seen, suggestion);

                    var insertable = false;

                    for (var i = before; i < result.Items.Count; i++)
                    {
                        if (!result.Items[i].Insertable)
                            continue;

                        insertable = true;

                        break;
                    }

                    if (insertable)
                        viable = true;

                    if (candidate.Validate(prefix, template, out var expectation) == ArgumentState.Invalid)
                    {
                        if (expected.Length == 0)
                            expected = expectation;
                    }
                    else
                        viable = true;

                    if (!insertable && result.Items.Count == before && candidate.AcceptsToken)
                        placeholders.Add(Suggestion.Create(null, 0, info.ReplaceStart, info.ReplaceLength, SuggestionKind.Placeholder, candidate.UsageText, candidate.UsageText));
                }

                if (viable)
                    accepted = true;
            }

            result.Header = usageParts.Count == 0 ? "/" + info.Command : "/" + info.Command + " " + string.Join(" | ", usageParts);

            if (!accepted)
            {
                if (expected.Length > 0)
                    result.Error = Language.ActiveCulture.LegacyId == 7 ? $"参数无效, 期望 {expected}" : $"Invalid argument, expected {expected}";
                else if (matched)
                    result.Error = Language.ActiveCulture.LegacyId == 7 ? "参数过多" : "Too many arguments";
            }

            if (result.Error.Length == 0 && !result.Items.Any(item => item.Insertable))
                result.Items.AddRange(placeholders);
        }

        private static void Add(List<Suggestion> items, HashSet<string> seen, Suggestion suggestion)
        {
            var key = (int)suggestion.Kind + "\u0000" + (suggestion.Insert ?? string.Empty) + "\u0000" + (suggestion.Display ?? string.Empty);

            if (seen.Add(key))
                items.Add(suggestion);
        }

        private static int Compare(Suggestion left, Suggestion right)
        {
            var score = Score(left).CompareTo(Score(right));

            if (score != 0)
                return score;

            return string.Compare(left.Display, right.Display, StringComparison.OrdinalIgnoreCase);
        }

        private static int Score(Suggestion suggestion)
        {
            if (suggestion.Kind is SuggestionKind.Placeholder or SuggestionKind.Hint)
                return 3;

            if (suggestion.Kind == SuggestionKind.Command)
                return 0;

            return suggestion.MatchLength > 0 ? 1 : 2;
        }
    }
}
