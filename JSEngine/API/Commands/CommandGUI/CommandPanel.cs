using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.JSEngine.API.Commands.Completion;
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

            if (ChatBox.Instance.TextBox.Active && (LastChatText != CurrentChatText || LastIndex != CurrentIndex))
            {
                var result = CommandCompleter.Complete(CommandInfo.Parse(CurrentChatText, CurrentIndex));

                CompletionsContainer.SetSuggestions(result.Items, result.Header, result.Footer, result.Error);

                LastChatText = CurrentChatText;

                LastIndex = CurrentIndex;
            }

            RelativePosition.Y = Main.screenHeight - (50 + CompletionsContainer.Height);

            base.Update(gameTime);
        }

        public void KeyJustPress(object sender, KeyEventArgs e)
        {
            if (!isCommandInputActive || !CompletionsContainer.HasSuggestions)
                return;

            switch (e.KeyCode)
            {
                case Keys.Tab:

                    ApplySuggestion(UserInput.Shift ? -1 : 1);

                    break;

                case Keys.Down:

                    CompletionsContainer.MoveSelection(1);

                    break;

                case Keys.Up:

                    CompletionsContainer.MoveSelection(-1);

                    break;

                case Keys.PageDown:

                    CompletionsContainer.MoveSelection(CompletionsContainer.VisibleRowCount);

                    break;

                case Keys.PageUp:

                    CompletionsContainer.MoveSelection(-CompletionsContainer.VisibleRowCount);

                    break;
            }
        }

        public void KeyKeepPress(object sender, KeyEventArgs e)
        {
            if (!isCommandInputActive || !CompletionsContainer.HasSuggestions)
                return;

            switch (e.KeyCode)
            {
                case Keys.Down:

                    Timer[0]++;

                    if (Timer[0] < 6)
                        return;

                    Timer[0] = 0;

                    CompletionsContainer.MoveSelection(1);

                    break;

                case Keys.Up:

                    Timer[1]++;

                    if (Timer[1] < 6)
                        return;

                    Timer[1] = 0;

                    CompletionsContainer.MoveSelection(-1);

                    break;
            }
        }

        private void ApplySuggestion(int step)
        {
            var container = CompletionsContainer;

            var suggestion = container.Selected;

            if (suggestion is null)
                return;

            if (!suggestion.Insertable)
            {
                container.MoveSelection(step);

                return;
            }

            var textBox = ChatBox.Instance.TextBox;

            var start = Math.Clamp(suggestion.ReplaceStart, 0, textBox.Text.Length);

            var length = Math.Clamp(suggestion.ReplaceLength, 0, textBox.Text.Length - start);

            var insert = suggestion.Insert;

            if (insert.Contains(' ') && !insert.StartsWith('"'))
                insert = "\"" + insert + "\"";

            if (container.InsertableCount == 1)
            {
                var after = start + length;

                if (after >= textBox.Text.Length || !char.IsWhiteSpace(textBox.Text[after]))
                    insert += " ";
            }

            textBox.ReplaceRange(start, length, insert);

            container.MoveSelection(step);
        }

        public override bool Visible => ChatText.StartsWith('/') && Main.drawingPlayerChat;
    }
}
