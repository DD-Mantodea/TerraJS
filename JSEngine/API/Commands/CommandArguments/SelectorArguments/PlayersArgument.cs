using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria;
using Terraria.Localization;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments
{
    public class PlayersArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            foreach (var player in Main.player)
            {
                if (!player.active)
                    continue;

                if (player.name == content)
                {
                    value = new List<Player> { player };

                    return true;
                }
            }

            if (Selector.RegExp.TryMatch(content, out var match))
            {
                var identifier = match.Groups[1].Value;

                if (!PlayerSelector.Selectors.TryGetValue(identifier, out var selector))
                    return false;

                var pSelector = new PlayerSelector(Main.LocalPlayer);

                var conditions = match.Groups[2].Value.Split(',').Select(s => s.Trim()).ToList();

                foreach (var condition in conditions)
                {
                    var parts = new Regex("(>|<|=|>=|<=|!=)").Split(condition);

                    if (parts.Length != 3)
                        continue;

                    pSelector.Conditions.Add(new SelectorCondition(parts[0], parts[2], parts[1]));
                }

                value = selector(pSelector);

                return true;
            }

            return value != null;
        }

        public override List<string> GetCompletions(CommandInfo commandInfo)
        {
            var regex = new Regex(@"(@[a-zA-Z0-9]+)\[(.*)\]?");

            if (regex.TryMatch(commandInfo.CurrentParameter, out var match) && commandInfo.State == InputState.Selector)
            {
                var parts = match.Groups[2].Value.Split(',');

                var currentInput = parts.Select(s => s.Trim()).Last();

                var cursorPos = commandInfo.RelativeCursorPosition;

                var length = match.Groups[1].Length + 1;

                foreach (var part in parts)
                {
                    if (cursorPos > length)
                        length += part.Length;

                    if (cursorPos <= length)
                    {
                        currentInput = part;

                        break;
                    }

                    length++;
                }

                return DealStartWith([.. typeof(Player).GetFields().Select(f => f.Name), .. typeof(Player).GetProperties().Select(p => p.Name)], currentInput);
            }
            else
                return DealStartWith([.. Main.player.Where(p => p.active).Select(p => p.name), .. PlayerSelector.Selectors.Keys.Select(s => $"@{s}")], commandInfo.CurrentParameter);
        }

        public override string ToString() => IsOptional ? $"[<{Name} : PlayerSelector | Player>]" : $"<{Name} : PlayerSelector | Player>";

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            if (context.InBracket && context.Info.State == InputState.Selector)
            {
                if (!context.InValue)
                {
                    foreach (var name in ConditionNames)
                    {
                        if (!name.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                            continue;

                        yield return context.Value(name, name, null, context.Prefix.Length);
                    }

                    yield break;
                }

                foreach (var entry in GetConditionValues(context.AttributeName))
                {
                    if (!entry.Id.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    yield return context.Value(entry.Id, entry.Display, entry.Description, context.Prefix.Length);
                }

                yield break;
            }

            foreach (var player in Main.player)
            {
                if (!player.active || !player.name.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(player.name, player.name, null, context.Prefix.Length);
            }

            foreach (var key in PlayerSelector.Selectors.Keys)
            {
                var selector = "@" + key;

                if (!selector.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(selector, selector, SelectorDescription(key), context.Prefix.Length);
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

            if (token[0] == '@')
            {
                var bracket = token.IndexOf('[');

                var head = (bracket < 0 ? token : token[..bracket])[1..];

                if (head.Length == 0)
                    return ArgumentState.Incomplete;

                foreach (var key in PlayerSelector.Selectors.Keys)
                {
                    if (key.StartsWith(head, StringComparison.OrdinalIgnoreCase))
                        return ArgumentState.Incomplete;
                }

                return ArgumentState.Invalid;
            }

            foreach (var player in Main.player)
            {
                if (player.active && player.name.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    return ArgumentState.Incomplete;
            }

            return ArgumentState.Invalid;
        }

        private static string SelectorDescription(string name)
        {
            var key = "Commands.Selector." + name;

            var text = TJSEngine.GlobalAPI.Translation.GetTranslation(key, Language.ActiveCulture);

            if (text == key)
                TJSEngine.GlobalAPI.Translation.DefaultLocalizedTexts.TryGetValue(key, out text);

            return text == key ? null : text;
        }

        private static List<string> _conditionNames;

        private static List<string> ConditionNames => _conditionNames ??= BuildConditionNames();

        private static List<string> BuildConditionNames()
        {
            var names = new List<string>();

            foreach (var field in typeof(Player).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var fullName = field.FieldType.FullName;

                if (fullName is not null && Selector.ConditionCheckers.ContainsKey(fullName))
                    names.Add(field.Name);
            }

            foreach (var property in typeof(Player).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var fullName = property.PropertyType.FullName;

                if (fullName is not null && Selector.ConditionCheckers.ContainsKey(fullName))
                    names.Add(property.Name);
            }

            names.Sort(StringComparer.OrdinalIgnoreCase);

            return names;
        }

        private static List<CompletionEntry> GetConditionValues(string name)
        {
            var entries = new List<CompletionEntry>();

            switch (name?.ToLowerInvariant())
            {
                case "name":

                    foreach (var player in Main.player)
                    {
                        if (player.active)
                            entries.Add(new CompletionEntry(player.name, player.name));
                    }

                    break;

                case "team":

                    for (var i = 0; i <= 4; i++)
                        entries.Add(new CompletionEntry(i.ToString(), i.ToString()));

                    break;
            }

            return entries;
        }

        public override bool InScope(object value, object last)
        {
            return value is List<Player> players && players.Count > 0;
        }
    }
}
