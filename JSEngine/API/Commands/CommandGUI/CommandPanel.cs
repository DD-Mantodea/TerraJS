using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using TerraJS.Contents.UI;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.UI.Components;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.Utils;

namespace TerraJS.API.Commands.CommandGUI
{
    [RegisterUI("CommandPanel")]
    public class CommandPanel : SizeContainer
    {
        public CommandPanel()
        {
            RelativePosition = new(78, 0);

            CompletionsContainer = new CompletionsContainer().Join(this);
        }

        public static CommandPanel Instance => UISystem.GetUIInstance<CommandPanel>("CommandPanel");

        public string LastChatText = "";

        public string CurrentChatText = "";

        public string ChatText => ChatBox.Instance.TextBox.Text;

        public CommandInfo CommandInfo => CommandInfo.Parse(ChatText, ChatBox.Instance.TextBox.Cursor.CursorIndex);

        public CompletionsContainer CompletionsContainer;

        public bool isCommandInputActive => ChatText.StartsWith('/') && ChatBox.Instance.TextBox.Active;

        public override void Update(GameTime gameTime)
        {
            CurrentChatText = ChatText;

            var container = CompletionsContainer;

            var completions = container.Completions;

            if (ChatBox.Instance.TextBox.Active && (LastChatText != CurrentChatText))
            {
                if (ChatText.StartsWith("/"))
                {
                    if (CommandInfo.State == InputState.Command)
                    {
                        var matchingCommands = container.GetMatchingCommands();

                        container.RemoveAllChild();

                        container.RebuildCompletions([.. matchingCommands.Select(command =>
                    {
                        var text = "";

                        var key = command.Command;

                        var match = CommandInfo.Command;

                        var dismatch = key[match.Length..];

                        if (command is TJSCommand tjscmd && tjscmd.TryGetArgumentsText([], out var args))
                            text = (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + dismatch + args;
                        else
                            text = (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + dismatch;

                        return text;
                    })]);
                    }
                    else
                    {
                        var commands = container.GetAvailableCommands().Where(c => c is TJSCommand && c.Command.StartsWith(CommandInfo.Command)).Select(c => c as TJSCommand).ToList();

                        var values = new List<string>();

                        var match = CommandInfo.CurrentParameter;

                        container.RemoveAllChild();

                        foreach (var command in commands)
                        {
                            var argsGroup = CommandAPI.CommandArgumentGroups[command.GetType().FullName];

                            if (argsGroup.Arguments.Count <= CommandInfo.ParameterIndex)
                                continue;

                            var arg = argsGroup.Arguments[CommandInfo.ParameterIndex];

                            var argCompletions = arg.GetCompletions();

                            if (argCompletions.Count > 0)
                                values.TryAddRange(argCompletions);
                            else
                                values.Add(arg.ToString());
                        }

                        container.RebuildCompletions(values.Where(t => t.StartsWith(match)).Select(t => (match.Length == 0 ? "" : $"[c/F4F32B:{match}]") + t[match.Length..]));
                    }

                    LastChatText = CurrentChatText;
                }
                else
                    container.RemoveAllChild();
            }

            if (isCommandInputActive)
            {
                if (completions.Count > 0)
                {
                    var index = container.SelectedCompletionIndex;

                    if (UserInput.IsJustPress(Keys.Tab))
                    {
                        var selected = SnippetUtils.GetPlainText(completions[index]);

                        if (selected.Contains(' '))
                            selected = selected.Split(" ")[0];

                        var currentInput = CommandInfo.State == InputState.Command ? CommandInfo.Command : CommandInfo.CurrentParameter;

                        if (currentInput != selected)
                            ChatBox.Instance.TextBox.AppendString(selected[currentInput.Length..] + " ");
                    }

                    if (UserInput.IsJustPress(Keys.Down))
                        container.SelectedCompletionIndex = (index + 1) % completions.Count;
                    else if (UserInput.IsJustPress(Keys.Up))
                        container.SelectedCompletionIndex = (index - 1 + completions.Count) % completions.Count;
                }
            }

            RelativePosition.Y = Main.screenHeight - (50 + CompletionsContainer.Height);

            base.Update(gameTime);
        }

        public override bool Visible => ChatText.StartsWith('/') && Main.drawingPlayerChat;
    }
}
