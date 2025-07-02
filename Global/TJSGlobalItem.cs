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
            TerraJS.GlobalAPI.Event.Recipe.AddRecipesEvent?.Invoke();
        }

        public override void UpdateInventory(Item item, Player player)
        {
            TerraJS.GlobalAPI.Event.Item.UpdateInventoryEvent?.Invoke(item, player);
        }

        public override void SetDefaults(Item item)
        {
            TerraJS.GlobalAPI.Event.Item.SetDefaultsEvent?.Invoke(item);
        }

        public override void RightClick(Item item, Player player)
        {
            TerraJS.GlobalAPI.Event.Item.RightClickEvent?.Invoke(item, player);
        }

        public override bool CanRightClick(Item item)
        {
            return TerraJS.GlobalAPI.Event.Item.CanRightClickEvent?.Invoke(item) ?? false;
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            return TerraJS.GlobalAPI.Event.Item.ConsumeItemEvent?.Invoke(item, player) ?? true;
        }

        public override bool? UseItem(Item item, Player player)
        {
            return TerraJS.GlobalAPI.Event.Item.UseItemEvent?.Invoke(item, player) ?? null;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return TerraJS.GlobalAPI.Event.Item.CanUseItemEvent?.Invoke(item, player) ?? true;
        }
    }
}
