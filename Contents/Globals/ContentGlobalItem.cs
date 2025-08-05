using System.Collections.Generic;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TerraJS.Contents.Globals
{
    public class ContentGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem == null)
                return;

            var lang = Language.ActiveCulture;

            tooltips.Add(new(TerraJS.Instance, "itemInternalName", $"{(lang.LegacyId == 7 ? "物品内部名称: " : "item internal name: ")}{item.ModItem.GetType().Name}") { OverrideColor = Color.Green });

            tooltips.Add(new(TerraJS.Instance, "modInternalName", $"{(lang.LegacyId == 7 ? "模组内部名称: " : "mod internal name: ")}{item.ModItem.Mod.Name}") { OverrideColor = Color.Green });
        }
    }
}
