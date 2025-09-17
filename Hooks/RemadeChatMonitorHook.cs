using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using TerraJS.Contents.UI.Chat;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    public class RemadeChatMonitorHook : ModSystem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(RemadeChatMonitor).GetMethod("AddNewMessage"), AddNewMessageHook);
        }

        private static void AddNewMessageHook(Action<RemadeChatMonitor, string, Color, int> orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
        {
            if (MessageBox.Instance is null || string.IsNullOrEmpty(text))
                return;

            var sender = MatchUsername.Match(text);

            if (sender.Success)
            {
                text = text[(sender.Value.Length + 1)..];

                var playerName = sender.Groups[1].Value;

                if (Main.LocalPlayer is { } localPlayer && localPlayer.name.Equals(playerName))
                    MessageBox.Instance.AppendMessage(text, playerName, color, true);
                else
                    MessageBox.Instance.AppendMessage(text, playerName, color, false);
            }
            else
            {
                MessageBox.Instance.AppendMessage(text, null, color, false);
            }
        }
        private static Regex MatchUsername => new Regex(@"^\[n:(.*?)\]");
    }
}
