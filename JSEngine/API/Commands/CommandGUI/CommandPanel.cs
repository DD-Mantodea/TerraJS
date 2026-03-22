using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Microsoft.Xna.Framework.Input;
using TerraJS.Contents.UI;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Attributes;
using System.Text.RegularExpressions;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.Utils;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
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

        public int LastIndex = 0;

        public int CurrentIndex = 0;

        public string ChatText => ChatBox.Instance.TextBox.Text;

        public CompletionsContainer CompletionsContainer;

        public bool isCommandInputActive => ChatText.StartsWith('/') && ChatBox.Instance.TextBox.Active;

        public override void Update(GameTime gameTime)
        {
            CurrentChatText = ChatText;

            CurrentIndex = ChatBox.Instance.TextBox.Cursor.CursorIndex;

            var container = CompletionsContainer;

            var completions = container.Completions;

            var commandInfo = CommandInfo.Parse(ChatText, ChatBox.Instance.TextBox.Cursor.CursorIndex);

            if (ChatBox.Instance.TextBox.Active && (LastChatText != CurrentChatText || LastIndex != CurrentIndex))
            {
                if (ChatText.StartsWith('/'))
                {
                    if (commandInfo.State == InputState.Command)
                    {
                        var matchingCommands = container.GetMatchingCommands();

                        container.RemoveAllChild();

                        container.RebuildCompletions([.. matchingCommands.Select(command =>
                        {
                            var text = "";

                            var key = command.Command;

                            var match = commandInfo.Command;

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
                        var commands = container.GetAvailableCommands().Where(c => c is TJSCommand && c.Command.StartsWith(commandInfo.Command)).Select(c => c as TJSCommand).ToList();

                        var values = new List<string>();

                        var match = commandInfo.CurrentParameter;

                        container.RemoveAllChild();

                        foreach (var command in commands)
                        {
                            var argsGroup = CommandAPI.CommandArgumentGroups[command.GetType().FullName];

                            if (argsGroup.Arguments.Count <= commandInfo.ParameterIndex)
                                continue;

                            var arg = argsGroup.Arguments[commandInfo.ParameterIndex];

                            var argCompletions = arg.GetCompletions(commandInfo);

                            if (argCompletions.Count > 0)
                                values.TryAddRange(argCompletions);
                            else
                                values.Add(arg.ToString());
                        }

                        container.RebuildCompletions(values);
                    }

                    LastChatText = CurrentChatText;

                    LastIndex = CurrentIndex;
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

                        if (!selected.StartsWith('<'))
                        {
                            var state = commandInfo.State;

                            var currentInput = state == InputState.Command ? commandInfo.Command : commandInfo.CurrentParameter;

                            if (state == InputState.Selector)
                            {
                                var match = new Regex(@"(@[a-zA-Z0-9]+)\[(.*)\]?").Match(currentInput);

                                var parts = match.Groups[2].Value.Split(',');

                                var cursorPos = commandInfo.RelativeCursorPosition;

                                var length = match.Groups[1].Length + 1;

                                foreach (var part in parts)
                                {
                                    if (cursorPos > length)
                                        length += part.Length;

                                    if (cursorPos <= length)
                                    {
                                        currentInput = part;

                                        break;
                                    }

                                    length++;
                                }

                                ChatBox.Instance.TextBox.AppendAt(selected[currentInput.Length..], length + commandInfo.CursorPosition - commandInfo.RelativeCursorPosition - 1);
                            }
                            else if (currentInput != selected)
                                ChatBox.Instance.TextBox.AppendString(selected[currentInput.Length..]);
                        }
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
