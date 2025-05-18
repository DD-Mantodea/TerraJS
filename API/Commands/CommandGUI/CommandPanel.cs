using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using SilkyUIFramework.BasicElements;
using Terraria.GameInput;
using Terraria;
using Terraria.ModLoader;
using SilkyUIFramework.Extensions;
using Terraria.GameContent;
using SilkyUIFramework.Attributes;
using SilkyUIFramework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TerraJS.API.Commands.CommandGUI
{
    [RegisterUI("Vanilla: Radial Hotbars", "TerraJS: CommandPanel", int.MaxValue)]
    public class CommandPanel : BasicBody
    {
        public static CommandPanel Instance;

        public string LastChatText = "";

        public string CurrentChatText = "";

        public string CurrentInput => Main.chatText[1..];

        public List<string> matchingCommands = [];

        public List<UITextView> CommandLines => ScrollView.Container.Children.Cast<UITextView>().ToList();

        public int selectedCommandIndex = 0;

        public SUIScrollView ScrollView;

        public bool isCommandInputActive => Main.chatText.StartsWith('/');

        public CommandPanel() => Instance = this;

        public void UpdateMatchingCommands()
        {
            var allCommands = GetAvailableCommands();
            matchingCommands = [.. allCommands
                .Where(cmd => cmd.StartsWith(CurrentInput, StringComparison.OrdinalIgnoreCase))
                .OrderBy(cmd => cmd)];

            selectedCommandIndex = 0;
        }

        public List<string> GetAvailableCommands()
        {
            List<string> commands = new List<string>();

            var allCommands = typeof(CommandLoader).GetField("Commands", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IDictionary<string, List<ModCommand>>;

            foreach (var commandEntry in allCommands)
            {
                // 只添加当前环境可用的命令（聊天命令）  
                foreach (var command in commandEntry.Value)
                {
                    if (CommandLoader.Matches(command.Type, CommandType.Chat))
                    {
                        commands.Add(commandEntry.Key);
                        break;
                    }
                }
            }

            return commands;
        }

        protected override void OnInitialize()
        {
            SetLeft(78);

            SetWidth(TextureAssets.TextBack.Width() - 100);

            SetGap(0);

            SetHeight(0);

            FitHeight = true;

            Border = 0;

            ScrollView = new SUIScrollView()
            {
                FitHeight = true,
                FlexDirection = FlexDirection.Column
            }.Join(this);

            ScrollView.SetGap(0);

            ScrollView.SetWidth(0, 1);

            ScrollView.Mask.FitHeight = true;

            ScrollView.Mask.SetWidth(0, 1);

            ScrollView.Container.FitHeight = true;

            ScrollView.Container.SetGap(2);

            ScrollView.Container.FlexDirection = FlexDirection.Column;

            ScrollView.Container.FlexWrap = false;

            ScrollView.Container.SetWidth(0, 1);

            ScrollView.Container.SetHeight(0);

            ScrollView.Container.SetMaxHeight(210);
        }

        public override void HandleUpdate(GameTime gameTime)
        {
            CurrentChatText = Main.chatText;

            if (Main.drawingPlayerChat && PlayerInput.WritingText && (LastChatText != CurrentChatText))
            {
                UpdateMatchingCommands();

                ScrollView.Container.RemoveAllChildren();

                foreach (var command in matchingCommands)
                {
                    var match = CurrentInput;

                    var dismatch = match.Length == 0 ? command : command.Replace(match, "");

                    var cmdLine = new UITextView()
                    {
                        Text = (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + dismatch,
                        BackgroundColor = Color.Gray * 0.3f,
                        Padding = 0,
                    }.Join(ScrollView.Container);

                    cmdLine.SetHeight(30);

                    cmdLine.FitWidth = false;

                    cmdLine.SetWidth(0, 1);

                    cmdLine.Padding = new Margin(4, 0, 4, 0);
                }

                LastChatText = CurrentChatText;
            }

            if (isCommandInputActive && matchingCommands.Count > 0)
            {
                if (Main.keyState.IsKeyDown(Keys.Tab) && !Main.oldKeyState.IsKeyDown(Keys.Tab))
                {
                    string selectedCommand = matchingCommands[selectedCommandIndex];
                    Main.chatText = "/" + selectedCommand + " ";
                }

                if (Main.keyState.IsKeyDown(Keys.Down) && !Main.oldKeyState.IsKeyDown(Keys.Down))
                {
                    selectedCommandIndex = (selectedCommandIndex + 1) % matchingCommands.Count;
                }
                else if (Main.keyState.IsKeyDown(Keys.Up) && !Main.oldKeyState.IsKeyDown(Keys.Up))
                {
                    selectedCommandIndex = (selectedCommandIndex - 1 + matchingCommands.Count) % matchingCommands.Count;
                }
            }

            base.HandleUpdate(gameTime);

            SetTop(Main.screenHeight - (50 + Bounds.Height));

            for(int i = 0; i < CommandLines.Count; i++)
            {
                if (selectedCommandIndex == i)
                    CommandLines[i].BackgroundColor = Color.LightGray * 0.3f;
                else
                    CommandLines[i].BackgroundColor = Color.Gray * 0.3f;
            }
        }

        public override bool Enabled => Main.chatText.StartsWith('/') && Main.drawingPlayerChat;
    }
}
