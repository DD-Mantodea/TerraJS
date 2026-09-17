using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria;

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

        public override bool InScope(object value, object last)
        {
            return Main.player.Contains(value);
        }
    }
}
