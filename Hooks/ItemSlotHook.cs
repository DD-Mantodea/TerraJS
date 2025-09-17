using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.UI.Chat;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraJS.Hooks
{
    public class ItemSlotHook : ModSystem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(ItemSlot).GetMethod("OverrideHover", [typeof(Item[]), typeof(int), typeof(int)]), OverrideHoverHook);
        }

        private static void OverrideHoverHook(Action<Item[], int, int> orig, Item[] inv, int context = 0, int slot = 0)
        {
            orig(inv, context, slot);

            if (Main.cursorOverride == 3 && ChatBox.Instance.Visible)
                Main.cursorOverride = 2;
        }
    }
}
