using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
{
    public class CompletionsContainer : ColumnContainer
    {
        public CompletionsContainer()
        {
            for (var i = 0; i < VisibleRowCount; i++)
            {
                var row = new SizeContainer(0, RowHeight)
                {
                    BackgroundColor = Color.Gray * 0.7f
                }.Join(this);

                var content = new UIText("Andy-Bold", fontSize: 22).Join(row);

                content.RelativePosition = new(8, 0);

                content.TextVerticalMiddle = true;

                content.VerticalMiddle = true;

                _rows.Add(row);

                _rowTexts.Add(content);
            }
        }

        public const int VisibleRowCount = 6;

        public const int RowHeight = 36;

        private readonly List<string> _completions = [];

        private readonly List<SizeContainer> _rows = [];

        private readonly List<UIText> _rowTexts = [];

        private int _contentVersion;

        private int _shownVersion = -1;

        private int _shownStart = -1;

        public int SelectedCompletionIndex = 0;

        public IReadOnlyList<string> Completions => _completions;

        public string ChatText => ChatBox.Instance.TextBox.Text;

        public string CurrentInput => ChatText.StartsWith("/") ? ChatText.Substring(1) : "";

        public string[] Args => CurrentInput.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        public void SetCompletions(IReadOnlyList<string> completions)
        {
            _completions.Clear();

            _completions.AddRange(completions);

            SelectedCompletionIndex = 0;

            _contentVersion++;
        }

        public List<ModCommand> GetMatchingCommands()
        {
            var allCommands = GetAvailableCommands();

            var matchingCommands = new List<ModCommand>();

            if (Args.Length <= 1)
                matchingCommands = [.. allCommands
                    .Where(cmd => cmd.Command.StartsWith(Args.Length == 0 ? "" : Args[0], StringComparison.OrdinalIgnoreCase))
                    .OrderBy(cmd => cmd.Command)];
            else
                matchingCommands = [.. allCommands
                    .Where(cmd => {
                        if(cmd is TJSCommand tjscmd)
                            return tjscmd.TryGetArgumentsText(Args[1..], out var _) && tjscmd.Command.StartsWith(Args[0], StringComparison.OrdinalIgnoreCase);
                        return false;
                    })
                    .OrderBy(cmd => cmd.Command)];

            SelectedCompletionIndex = 0;

            return matchingCommands;
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

        private int WindowStart => SelectedCompletionIndex <= VisibleRowCount - 1 ? 0 : SelectedCompletionIndex - (VisibleRowCount - 1);

        private int WindowCount => Math.Clamp(_completions.Count - WindowStart, 0, VisibleRowCount);

        private void RefreshRows()
        {
            var start = WindowStart;

            var count = WindowCount;

            var maxWidth = 300;

            for (var i = 0; i < count; i++)
            {
                var content = _rowTexts[i];

                content.SetSnippets(SnippetUtils.ParseMessage(_completions[start + i]));

                maxWidth = Math.Max(maxWidth, content.Width + 8);
            }

            maxWidth = Math.Min(maxWidth, Math.Max(300, Main.screenWidth - 120));

            for (var i = 0; i < count; i++)
                _rows[i].SetSize(maxWidth, RowHeight);

            _shownVersion = _contentVersion;

            _shownStart = start;
        }

        public override void DrawChildren(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (var i = 0; i < VisibleRowCount; i++)
            {
                if (_rows[i].Visible)
                    _rows[i].DrawSelf(spriteBatch, gameTime);
            }
        }

        public override void UpdateChildren(GameTime gameTime)
        {
            _width = 0;

            _height = 0;

            var start = WindowStart;

            var count = WindowCount;

            if (_shownVersion != _contentVersion || _shownStart != start)
                RefreshRows();

            for (var i = 0; i < VisibleRowCount; i++)
            {
                var row = _rows[i];

                row.Visible = i < count;

                if (!row.Visible)
                    continue;

                SetChildRelativePos(row);

                row.Update(gameTime);

                row.BackgroundColor = start + i == SelectedCompletionIndex ? Color.LightGray * 0.7f : Color.Gray * 0.7f;
            }
        }
    }
}
