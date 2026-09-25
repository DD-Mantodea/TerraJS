using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.JSEngine.API.CommandArguments.SelectorArguments;
using Terraria;
using Terraria.Localization;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors
{
    public static class PlayerSelector
    {
        public static SelectorRegistry<Player> Registry => SelectorRegistry<Player>.Instance;

        public static Dictionary<string, SelectorEntry<Player>> Selectors => Registry.Selectors;

        static PlayerSelector()
        {
            RegisterTranslations();

            Registry.Register("p", new SelectorEntry<Player>(Candidates, SelectorSort.Nearest, 1));

            Registry.Register("r", new SelectorEntry<Player>(Candidates, SelectorSort.Random, 1));

            Registry.Register("a", new SelectorEntry<Player>(Candidates, SelectorSort.None, -1));

            Registry.Register("s", new SelectorEntry<Player>(Self, SelectorSort.None, -1));
        }

        public static List<Player> Candidates(Selector selector)
        {
            return [.. Main.player.Where(player => player.active && SelectorCondition.CheckAllTarget(player, selector))];
        }

        public static List<Player> Self(Selector selector)
        {
            if (selector.Sender is not Player player || !player.active || !SelectorCondition.CheckAllTarget(player, selector))
                return [];

            return [player];
        }

        private static void RegisterTranslations()
        {
            SetDefaultTranslation("p", "Nearest player", "最近的玩家");

            SetDefaultTranslation("r", "Random player", "随机玩家");

            SetDefaultTranslation("a", "All players", "所有玩家");

            SetDefaultTranslation("s", "Self", "自己");
        }

        private static void SetDefaultTranslation(string name, string english, string chinese)
        {
            var translation = TJSEngine.GlobalAPI?.Translation;

            if (translation is null)
                return;

            var key = "Commands.Selector." + name;

            translation.SetDefaultTranslation(GameCulture.DefaultCulture, key, english);

            translation.SetDefaultTranslation(GameCulture.FromLegacyId(7), key, chinese);
        }
    }
}
