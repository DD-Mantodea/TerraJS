using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.Contents.Extensions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors
{
    public class PlayerSelector(Player player) : Selector(player), ISelector<PlayerSelector, Player>
    {
        public static Dictionary<string, Func<PlayerSelector, List<Player>>> Selectors { get; } = [];

        static PlayerSelector()
        {
            RegisterTranslations();

            Selectors.Add("p", (selector) =>
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (SelectorCondition.CheckAll(Main.LocalPlayer, selector.Conditions))
                        return [Main.LocalPlayer];
                    else
                        return [];
                }

                var players = Main.player.Where(p => p.active && SelectorCondition.CheckAll(p, selector.Conditions)).ToList();

                if (players.Count == 0)
                    return [];

                return [players.OrderBy(p => Vector2.DistanceSquared(p.position, selector.Sender.position)).First()];
            });

            Selectors.Add("r", (selector) =>
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (SelectorCondition.CheckAll(Main.LocalPlayer, selector.Conditions))
                        return [Main.LocalPlayer];
                    else
                        return [];
                }

                var players = Main.player.Where(p => p.active && SelectorCondition.CheckAll(p, selector.Conditions)).ToList();

                if (players.Count == 0)
                    return [];

                return [ players.Random() ];
            });

            Selectors.Add("a", (selector) =>
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (SelectorCondition.CheckAll(Main.LocalPlayer, selector.Conditions))
                        return [Main.LocalPlayer];
                    else
                        return [];
                }

                return [.. Main.player.Where(p => p.active && SelectorCondition.CheckAll(p, selector.Conditions))];
            }); 
            
            Selectors.Add("s", (selector) =>
            {
                return [ selector.Sender as Player ];
            });
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
