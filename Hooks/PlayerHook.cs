using System;
using System.Reflection;
using TerraJS.Contents.UI.Chat;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    public class PlayerHook : ModSystem
    {
        public static bool ShouldDisableOpenInventory = false;

        public static bool ShouldDisableScrollHotbar = false;

        public override void Load()
        {
            MonoModHooks.Add(typeof(Player).GetMethod("OpenInventory", BindingFlags.Static | BindingFlags.NonPublic), OpenInventoryHook);

            MonoModHooks.Add(typeof(Player).GetMethod("ScrollHotbar"), ScrollHotbarHook);
        }

        public static void OpenInventoryHook(Action orig)
        {
            if (ChatBox.Instance.JustClose || ShouldDisableOpenInventory)
                return;

            orig();
        }

        public static void ScrollHotbarHook(Action<Player, int> orig, Player self, int offset)
        {
            if (ShouldDisableScrollHotbar)
                return;

            orig(self, offset);
        }
    }
}
