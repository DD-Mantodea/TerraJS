using System;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Chat.TextSnippnts;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace TerraJS.Hooks
{
    public class ChatManagerHook : ModSystem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(ChatManager).GetMethod("AddChatText"), AddChatTextHook);
        }

        private static bool AddChatTextHook(Func<DynamicSpriteFont, string, Vector2, bool> orig, DynamicSpriteFont font, string text, Vector2 baseScale)
        {
            ChatBox.Instance.TextBox.AppendString(text);

            return true;
        }
    }
}
