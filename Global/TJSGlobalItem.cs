using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Global
{
    public class TJSGlobalItem : GlobalItem
    {
        public override void AddRecipes()
        {
            TerraJS.GlobalAPI.Event.InvokeEvent("AddRecipes");
        }

        public override void SetDefaults(Item item)
        {
            TerraJS.GlobalAPI.Event.Item.InvokeEvent("SetDefaults", item);
        }

        public override void RightClick(Item item, Player player)
        {
            TerraJS.GlobalAPI.Event.Item.InvokeEvent("RightClick", item, player);
        }

        public override bool CanRightClick(Item item)
        {
            return TerraJS.GlobalAPI.Event.Item.InvokeBoolEvent("CanRightClick", item).Value;
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            return TerraJS.GlobalAPI.Event.Item.InvokeBoolEvent("ConsumeItem", item).Value;
        }

        public override bool? UseItem(Item item, Player player)
        {
            return TerraJS.GlobalAPI.Event.Item.InvokeBoolEvent("UseItem", item, player);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return TerraJS.GlobalAPI.Event.Item.InvokeBoolEvent("CanUseItem", item, player).Value;
        }
    }
}
