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
            TerraJS.GlobalAPI.Event.InvokeEvent("AddRecipes", []);
            base.AddRecipes();
        }

        public override void UpdateInventory(Item item, Player player)
        {
            base.UpdateInventory(item, player);
        }
    }
}
