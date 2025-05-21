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

        public string[] Args => CurrentInput.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        public List<ModCommand> MatchingCommands = [];

        public List<UITextView> CommandLines => ScrollView.Container.Children.Cast<UITextView>().ToList();

        public int selectedCommandIndex = 0;

        public SUIScrollView ScrollView;

        public bool isCommandInputActive => Main.chatText.StartsWith('/');

        public CommandPanel() => Instance = this;

        public void UpdateMatchingCommands()
        {
            var allCommands = GetAvailableCommands();

            if (Args.Count() <= 1)
                MatchingCommands = [.. allCommands
                    .Where(cmd => cmd.Command.StartsWith(Args.Length == 0 ? "" : Args[0], StringComparison.OrdinalIgnoreCase))
                    .OrderBy(cmd => cmd.Command)];
            else
                MatchingCommands = [.. allCommands
                    .Where(cmd => {
                        if(cmd is TJSCommand tjscmd)
                            return tjscmd.TryGetArgumentsText(Args[1..], out var _) && tjscmd.Command.StartsWith(Args[0], StringComparison.OrdinalIgnoreCase);
                        return false;
                    })
                    .OrderBy(cmd => cmd.Command)];

            MatchingCommands = MatchingCommands.Count > 7 ? MatchingCommands[0..6] : MatchingCommands;

            selectedCommandIndex = 0;
        }

        public List<ModCommand> GetAvailableCommands()
        {
            List<ModCommand> commands = [];

            var allCommands = typeof(CommandLoader).GetField("Commands", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IDictionary<string, List<ModCommand>>;

            foreach (var list in allCommands.Values)
            {
                foreach (var command in list)
                {
                    if (CommandLoader.Matches(command.Type, CommandType.Chat))
                        commands.Add(command);
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

            ScrollView.Container.SetMaxHeight(208);
        }

        public override void HandleUpdate(GameTime gameTime)
        {
            CurrentChatText = Main.chatText;

            if (Main.drawingPlayerChat && PlayerInput.WritingText && (LastChatText != CurrentChatText))
            {
                UpdateMatchingCommands();

                ScrollView.Container.RemoveAllChildren();

                foreach (var command in MatchingCommands)
                {
                    var text = "";

                    var key = command.Command;

                    var match = Args.Length == 0 ? "" : Args[0];

                    var dismatch = match.Length == 0 ? key : key.Replace(match, "");

                    if (command is TJSCommand tjscmd && tjscmd.TryGetArgumentsText(Args.Length <= 1 ? [] : Args[1..], out var argsText))
                        text = (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + dismatch + argsText;
                    else
                        text = (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + dismatch;

                    var cmdLine = new UITextView()
                    {
                        Text = text,
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

            if (isCommandInputActive && MatchingCommands.Count > 0)
            {
                if (Main.keyState.IsKeyDown(Keys.Tab) && !Main.oldKeyState.IsKeyDown(Keys.Tab))
                {
                    string selectedCommand = MatchingCommands[selectedCommandIndex].Command;

                    if((Args.Length == 0 ? "" : Args[0]) != selectedCommand)
                        Main.chatText = "/" + selectedCommand + " ";
                }

                if (Main.keyState.IsKeyDown(Keys.Down) && !Main.oldKeyState.IsKeyDown(Keys.Down))
                {
                    selectedCommandIndex = (selectedCommandIndex + 1) % MatchingCommands.Count;
                }
                else if (Main.keyState.IsKeyDown(Keys.Up) && !Main.oldKeyState.IsKeyDown(Keys.Up))
                {
                    selectedCommandIndex = (selectedCommandIndex - 1 + MatchingCommands.Count) % MatchingCommands.Count;
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
