using Jint.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using Terraria;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
{
    [RegisterUI("CommandPanel", priority: -1)]
    public class CommandPanel : SizeContainer
    {
        public CommandPanel()
        {
            RelativePosition = new(78, 0);

            CompletionsContainer = new CompletionsContainer().Join(this);

            UserInput.KeyJustPress += KeyJustPress;
            UserInput.KeyKeepPress += KeyKeepPress;

            Timer = new(2);
        }

        public static CommandPanel Instance => UISystem.GetUIInstance<CommandPanel>("CommandPanel");

        public string LastChatText = "";

        public string CurrentChatText = "";

        public int LastIndex = 0;

        public int CurrentIndex = 0;

        public string ChatText => ChatBox.Instance.TextBox.Text;

        public Timer Timer;

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

                        container.SetCompletions([.. matchingCommands.Select(command =>
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

                        container.SetCompletions(values);
                    }
                }
                else
                    container.SetCompletions([]);

                LastChatText = CurrentChatText;

                LastIndex = CurrentIndex;
            }

            RelativePosition.Y = Main.screenHeight - (50 + CompletionsContainer.Height);

            base.Update(gameTime);
        }

        public void KeyJustPress(object sender, KeyEventArgs e)
        {
            if (isCommandInputActive && CompletionsContainer.Completions.Count > 0)
            {
                var container = CompletionsContainer;

                var completions = container.Completions;

                var commandInfo = CommandInfo.Parse(ChatText, ChatBox.Instance.TextBox.Cursor.CursorIndex);

                var index = container.SelectedCompletionIndex;

                switch (e.KeyCode)
                {
                    case Keys.Tab:
                        var selected = SnippetUtils.GetPlainText(completions[index]);

                        if (selected.Contains(' '))
                            selected = selected.Split(" ")[0];

                        if (!selected.StartsWith('<'))
                        {
                            var state = commandInfo.State;

                            var currentInput = state == InputState.Command ? commandInfo.Command : commandInfo.CurrentParameter;

                            switch (state)
                            {
                                case InputState.Selector:
                                case InputState.Entity:

                                    Match match = null;

                                    switch (state)
                                    {
                                        case InputState.Selector:
                                            match = new Regex(@"(@[a-zA-Z0-9]+)\[(.*)\]?").Match(currentInput);

                                            break;
                                        case InputState.Entity:
                                            match = new Regex(@"([a-zA-Z0-9_]+:[a-zA-Z0-9_]+)\[(.*)\]?").Match(currentInput);

                                            break;
                                    }

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

                                    ChatBox.Instance.TextBox.AppendAt(selected[currentInput.Length..], length + commandInfo.CursorPosition - commandInfo.RelativeCursorPosition);

                                    ChatBox.Instance.TextBox.CursorTo(length + commandInfo.CursorPosition);

                                    break;
                                default:
                                    if (currentInput != selected)
                                        ChatBox.Instance.TextBox.AppendString(selected[currentInput.Length..]);

                                    break;
                            }
                        }

                        break;
                    case Keys.Down:
                        container.SelectedCompletionIndex = (index + 1) % completions.Count;

                        break;
                    case Keys.Up:
                        container.SelectedCompletionIndex = (index - 1 + completions.Count) % completions.Count;

                        break;
                }
            }
        }

        public void KeyKeepPress(object sender, KeyEventArgs e)
        {
            if (isCommandInputActive && CompletionsContainer.Completions.Count > 0)
            {
                var container = CompletionsContainer;

                var completions = container.Completions;

                var index = container.SelectedCompletionIndex;

                switch (e.KeyCode)
                {
                    case Keys.Down:
                        Timer[0]++;

                        if (Timer[0] < 6)
                            return;

                        Timer[0] = 0;

                        container.SelectedCompletionIndex = (index + 1) % completions.Count;

                        break;

                    case Keys.Up:
                        Timer[1]++;

                        if (Timer[1] < 6)
                            return;

                        Timer[1] = 0;

                        container.SelectedCompletionIndex = (index - 1 + completions.Count) % completions.Count;

                        break;
                }
            }
        }

        public override bool Visible => ChatText.StartsWith('/') && Main.drawingPlayerChat;
    }
}
