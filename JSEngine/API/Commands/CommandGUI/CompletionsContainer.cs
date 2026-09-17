using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Chat.TextSnippnts;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
{
    public class CompletionsContainer : ColumnContainer
    {
        public CompletionsContainer()
        {
            _errorRow = CreateRow(ErrorColor);

            _errorText = CreateText(_errorRow);

            _headerRow = CreateRow(HeaderColor);

            _headerText = CreateText(_headerRow);

            for (var i = 0; i < VisibleRowCount; i++)
            {
                var row = CreateRow(RowColor);

                _rows.Add(row);

                _rowTexts.Add(CreateText(row));
            }

            _descriptionRow = CreateRow(FooterColor);

            _descriptionText = CreateText(_descriptionRow);

            _footerRow = CreateRow(FooterColor);

            _footerText = CreateText(_footerRow);

            _errorRow.Visible = false;

            _headerRow.Visible = false;

            _descriptionRow.Visible = false;

            _footerRow.Visible = false;
        }

        public const int VisibleRowCount = 6;

        public const int RowHeight = 36;

        private static readonly Color RowColor = Color.Gray * 0.7f;

        private static readonly Color SelectedColor = Color.LightGray * 0.7f;

        private static readonly Color HeaderColor = Color.Black * 0.75f;

        private static readonly Color FooterColor = Color.Black * 0.6f;

        private static readonly Color HighlightColor = ColorUtils.FromHex(0xF4F32B);

        private static readonly Color HintColor = ColorUtils.FromHex(0x9AA0A6);

        private static readonly Color ErrorColor = ColorUtils.FromHex(0x4A1010);

        private static readonly Color ErrorTextColor = ColorUtils.FromHex(0xFF6B6B);

        private readonly List<Suggestion> _suggestions = [];

        private readonly List<SizeContainer> _rows = [];

        private readonly List<UIText> _rowTexts = [];

        private readonly SizeContainer _errorRow;

        private readonly UIText _errorText;

        private readonly SizeContainer _headerRow;

        private readonly UIText _headerText;

        private readonly SizeContainer _descriptionRow;

        private readonly UIText _descriptionText;

        private readonly SizeContainer _footerRow;

        private readonly UIText _footerText;

        private string _header = string.Empty;

        private string _footer = string.Empty;

        private string _error = string.Empty;

        private int _contentVersion;

        private int _shownVersion = -1;

        private int _shownStart = -1;

        private int _shownSelected = -1;

        public int SelectedCompletionIndex;

        public IReadOnlyList<Suggestion> Completions => _suggestions;

        public bool HasSuggestions => _suggestions.Count > 0;

        public Suggestion Selected => _suggestions.Count == 0 ? null : _suggestions[Math.Clamp(SelectedCompletionIndex, 0, _suggestions.Count - 1)];

        public int InsertableCount
        {
            get
            {
                var count = 0;

                foreach (var suggestion in _suggestions)
                {
                    if (suggestion.Insertable)
                        count++;
                }

                return count;
            }
        }

        public void SetSuggestions(IReadOnlyList<Suggestion> suggestions, string header = null, string footer = null, string error = null)
        {
            var keepSelection = _suggestions.Count == suggestions.Count;

            if (keepSelection)
            {
                for (var i = 0; i < suggestions.Count; i++)
                {
                    if (_suggestions[i].Insert != suggestions[i].Insert)
                    {
                        keepSelection = false;

                        break;
                    }
                }
            }

            _suggestions.Clear();

            _suggestions.AddRange(suggestions);

            _header = header ?? string.Empty;

            _footer = footer ?? string.Empty;

            _error = error ?? string.Empty;

            SelectedCompletionIndex = keepSelection ? Math.Clamp(SelectedCompletionIndex, 0, Math.Max(0, _suggestions.Count - 1)) : 0;

            _contentVersion++;
        }

        public void Clear() => SetSuggestions([]);

        public void MoveSelection(int step)
        {
            if (_suggestions.Count == 0)
                return;

            SelectedCompletionIndex = Wrap(SelectedCompletionIndex + step, _suggestions.Count);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_suggestions.Count == 0 || !IsHovering)
                return;

            var delta = UserInput.GetDeltaWheelValue();

            if (delta != 0)
                MoveSelection(delta > 0 ? -1 : 1);
        }

        public override void DrawChildren(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_errorRow.Visible)
                _errorRow.DrawSelf(spriteBatch, gameTime);

            if (_headerRow.Visible)
                _headerRow.DrawSelf(spriteBatch, gameTime);

            for (var i = 0; i < VisibleRowCount; i++)
            {
                if (_rows[i].Visible)
                    _rows[i].DrawSelf(spriteBatch, gameTime);
            }

            if (_descriptionRow.Visible)
                _descriptionRow.DrawSelf(spriteBatch, gameTime);

            if (_footerRow.Visible)
                _footerRow.DrawSelf(spriteBatch, gameTime);
        }

        public override void UpdateChildren(GameTime gameTime)
        {
            _width = 0;

            _height = 0;

            var start = WindowStart;

            var count = WindowCount;

            if (_shownVersion != _contentVersion || _shownStart != start || _shownSelected != SelectedCompletionIndex)
                RefreshRows();

            if (_errorRow.Visible)
            {
                SetChildRelativePos(_errorRow);

                _errorRow.Update(gameTime);
            }

            if (_headerRow.Visible)
            {
                SetChildRelativePos(_headerRow);

                _headerRow.Update(gameTime);
            }

            for (var i = 0; i < VisibleRowCount; i++)
            {
                var row = _rows[i];

                row.Visible = i < count;

                if (!row.Visible)
                    continue;

                SetChildRelativePos(row);

                row.Update(gameTime);

                row.BackgroundColor = start + i == SelectedCompletionIndex ? SelectedColor : RowColor;
            }

            if (_descriptionRow.Visible)
            {
                SetChildRelativePos(_descriptionRow);

                _descriptionRow.Update(gameTime);
            }

            if (_footerRow.Visible)
            {
                SetChildRelativePos(_footerRow);

                _footerRow.Update(gameTime);
            }
        }

        private int WindowStart => SelectedCompletionIndex <= VisibleRowCount - 1 ? 0 : SelectedCompletionIndex - (VisibleRowCount - 1);

        private int WindowCount => Math.Clamp(_suggestions.Count - WindowStart, 0, VisibleRowCount);

        private void RefreshRows()
        {
            var start = WindowStart;

            var count = WindowCount;

            _errorRow.Visible = _error.Length > 0;

            _headerRow.Visible = _header.Length > 0;

            var description = Selected?.Description ?? string.Empty;

            _descriptionRow.Visible = description.Length > 0;

            _footerRow.Visible = _footer.Length > 0;

            _headerText.SetSnippets([new PlainTextSnippet(Trim(_header))]);

            _errorText.SetSnippets([new ColorTextSnippet(Trim(_error), Trim(_error), ErrorTextColor)]);

            _descriptionText.SetSnippets([new PlainTextSnippet(Trim(description))]);

            _footerText.SetSnippets([new PlainTextSnippet(Trim(_footer))]);

            var maxWidth = Math.Max(300, _headerText.Width + 8);

            maxWidth = Math.Max(maxWidth, _errorText.Width + 8);

            maxWidth = Math.Max(maxWidth, _descriptionText.Width + 8);

            maxWidth = Math.Max(maxWidth, _footerText.Width + 8);

            for (var i = 0; i < count; i++)
            {
                var content = _rowTexts[i];

                content.SetSnippets(BuildSnippets(_suggestions[start + i]));

                maxWidth = Math.Max(maxWidth, content.Width + 8);
            }

            maxWidth = Math.Min(maxWidth, Math.Max(300, Main.screenWidth - 120));

            _headerRow.SetSize(maxWidth, RowHeight);

            _errorRow.SetSize(maxWidth, RowHeight);

            _descriptionRow.SetSize(maxWidth, RowHeight);

            _footerRow.SetSize(maxWidth, RowHeight);

            for (var i = 0; i < count; i++)
                _rows[i].SetSize(maxWidth, RowHeight);

            _shownVersion = _contentVersion;

            _shownStart = start;

            _shownSelected = SelectedCompletionIndex;
        }

        private SizeContainer CreateRow(Color color)
        {
            return new SizeContainer(0, RowHeight)
            {
                BackgroundColor = color
            }.Join(this);
        }

        private static UIText CreateText(SizeContainer row)
        {
            var content = new UIText("Andy-Bold", fontSize: 22).Join(row);

            content.RelativePosition = new(8, 0);

            content.TextVerticalMiddle = true;

            content.VerticalMiddle = true;

            return content;
        }

        private static List<TextSnippet> BuildSnippets(Suggestion suggestion)
        {
            var display = suggestion.Display ?? string.Empty;

            if (!suggestion.Insertable)
                return [new ColorTextSnippet(display, display, HintColor)];

            var highlight = Math.Clamp(suggestion.MatchLength, 0, display.Length);

            List<TextSnippet> snippets = [];

            if (highlight > 0)
                snippets.Add(new ColorTextSnippet(display[..highlight], display[..highlight], HighlightColor));

            if (highlight < display.Length)
                snippets.Add(new PlainTextSnippet(display[highlight..]));

            if (snippets.Count == 0)
                snippets.Add(new PlainTextSnippet(string.Empty));

            return snippets;
        }

        private static int Wrap(int index, int count) => count <= 0 ? 0 : (index % count + count) % count;

        private static string Trim(string text) => string.IsNullOrEmpty(text) || text.Length <= 80 ? text : text[..80] + "…";
    }
}
