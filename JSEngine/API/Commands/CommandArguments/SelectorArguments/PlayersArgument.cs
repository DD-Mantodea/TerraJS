using System;
using System.Collections.Generic;
using TerraJS.JSEngine.API.CommandArguments.SelectorArguments;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria;
using Terraria.Localization;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments
{
    public class PlayersArgument(string name, bool isOptional = false) : SelectorArgument<Player>(name, isOptional)
    {
        public override SelectorRegistry<Player> Registry => PlayerSelector.Registry;

        public override string NameHint => Localized("玩家名", "player name");

        public override string ToString() => IsOptional ? $"[<{Name} : PlayerSelector | Player>]" : $"<{Name} : PlayerSelector | Player>";

        public override string Describe(string selectorName) => SelectorDescription(selectorName);

        public override bool TryResolveName(string text, out List<Player> targets)
        {
            targets = [];

            Player exact = null;

            Player loose = null;

            foreach (var player in Main.player)
            {
                if (!player.active || player.name.Length == 0)
                    continue;

                if (player.name == text)
                {
                    exact = player;

                    break;
                }

                if (loose is null && string.Equals(player.name, text, StringComparison.OrdinalIgnoreCase))
                    loose = player;
            }

            var match = exact ?? loose;

            if (match is null)
                return false;

            targets = [match];

            return true;
        }

        public override List<CompletionEntry> GetConditionValues(string conditionName)
        {
            if (string.Equals(conditionName, "name", StringComparison.OrdinalIgnoreCase))
            {
                var names = new List<CompletionEntry>();

                foreach (var player in Main.player)
                {
                    if (player.active && player.name.Length > 0)
                        names.Add(new CompletionEntry(player.name, player.name));
                }

                return names;
            }

            if (string.Equals(conditionName, "team", StringComparison.OrdinalIgnoreCase))
            {
                var teams = new List<CompletionEntry>();

                for (var i = 0; i <= 5; i++)
                    teams.Add(new CompletionEntry(i.ToString(), i.ToString()));

                return teams;
            }

            return base.GetConditionValues(conditionName);
        }

        protected override IEnumerable<CompletionEntry> NameCompletions()
        {
            foreach (var player in Main.player)
            {
                if (player.active && player.name.Length > 0)
                    yield return new CompletionEntry(player.name, player.name);
            }
        }

        private static string SelectorDescription(string name)
        {
            var key = "Commands.Selector." + name;

            var translation = TJSEngine.GlobalAPI?.Translation;

            if (translation is null)
                return null;

            var text = translation.GetTranslation(key, Language.ActiveCulture);

            if (text == key)
                translation.DefaultLocalizedTexts.TryGetValue(key, out text);

            return text == key ? null : text;
        }
    }
}
