using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.Selectors
{
    public class PlayerSelector(Entity sender) : Selector(sender)
    {
        public static Dictionary<string, Func<PlayerSelector, List<Player>>> Selectors = [];

        static PlayerSelector()
        {
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
        }
    }
}
