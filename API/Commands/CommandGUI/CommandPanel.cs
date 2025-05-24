using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Attributes;
using TerraJS.UI.Components.Containers;
using TerraJS.Extensions;
using Terraria.ModLoader;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.GameContent;
using TerraJS.UI.Components;

namespace TerraJS.API.Commands.CommandGUI
{
    [RegisterUI("CommandPanel")]
    public class CommandPanel : SizeContainer
    {
        public CommandPanel()
        {
            Instance = this;

            RelativePosition = new(78, 0);

            _width = TextureAssets.TextBack.Width() - 100;

            Column = new ColumnContainer().Join(this);
        }

        public static CommandPanel Instance;

        public string LastChatText = "";

        public string CurrentChatText = "";

        public string CurrentInput => Main.chatText.StartsWith('/') ? Main.chatText.Substring(1) : "";

        public string[] Args => CurrentInput.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        public List<ModCommand> MatchingCommands = [];

        public List<SizeContainer> CommandLines => Column.Children.Cast<SizeContainer>().ToList();

        public int selectedCommandIndex = 0;

        public ColumnContainer Column;

        public bool isCommandInputActive => Main.chatText.StartsWith('/');

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

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            CurrentChatText = Main.chatText;

            if (Main.drawingPlayerChat && PlayerInput.WritingText && (LastChatText != CurrentChatText))
            {
                UpdateMatchingCommands();

                Column.RemoveAllChild();

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

                    var cmdLine = new SizeContainer(Width, 36)
                    {
                        BackgroundColor = Color.Gray * 0.3f,
                    }.Join(Column);

                    var content = new UIText("MouseText", text: text, fontSize: 25).Join(cmdLine);

                    content.RelativePosition = new(4, 0);

                    content.TextVerticalMiddle = true;

                    content.VerticalMiddle = true;
                }

                LastChatText = CurrentChatText;
            }

            if (isCommandInputActive && MatchingCommands.Count > 0)
            {
                if (Main.keyState.IsKeyDown(Keys.Tab) && !Main.oldKeyState.IsKeyDown(Keys.Tab))
                {
                    string selectedCommand = MatchingCommands[selectedCommandIndex].Command;

                    if ((Args.Length == 0 ? "" : Args[0]) != selectedCommand)
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

            RelativePosition.Y = Main.screenHeight - (50 + Column.Height);

            for (int i = 0; i < CommandLines.Count; i++)
            {
                if (selectedCommandIndex == i)
                    CommandLines[i].BackgroundColor = Color.LightGray * 0.3f;
                else
                    CommandLines[i].BackgroundColor = Color.Gray * 0.3f;
            }

            base.Update(gameTime);
        }

        public override bool Visible => Main.chatText.StartsWith('/') && Main.drawingPlayerChat;
    }
}
