using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.IME;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    public class MainHook : ModSystem
    {
        public static bool ShouldDisableViewZoom = false;

        public override void Load()
        {
            var mainType = typeof(Main);

            MonoModHooks.Add(mainType.GetMethod("DoUpdate_HandleChat", BindingFlags.Static | BindingFlags.NonPublic), HandleChatHook);

            MonoModHooks.Add(mainType.GetMethod("DoUpdate_Enter_ToggleChat", BindingFlags.Static | BindingFlags.NonPublic), ToggleChatHook);

            MonoModHooks.Add(mainType.GetMethod("DrawPlayerChat", BindingFlags.Instance | BindingFlags.NonPublic), DrawPlayerChatHook);

            MonoModHooks.Add(mainType.GetMethod("UpdateViewZoomKeys"), UpdateViewZoomKeysHook);
        }

        public static void HandleChatHook(Action orig)
        {

        }

        public static void ToggleChatHook(Action orig)
        {
            Main.chatRelease = true;
        }

        public static void DrawPlayerChatHook(Action<Main> orig, Main self)
        {

        }

        public static void UpdateViewZoomKeysHook(Action<Main> orig, Main self)
        {
            if (!TextBox.HasActiveInstance || ShouldDisableViewZoom)
                orig(self);
        }
    }
}
