using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.Extensions;
using TerraJS.Utils;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace TerraJS.UI.Components
{
    public class UIText : Component
    {
        public UIText(string fontID, Vector2 size = default, string text = "", int fontSize = 20, int newLineNum = int.MaxValue,
            bool textHorizontalMiddle = false, bool textVerticalMiddle = false, Color? fontColor = null, string splitCharacter = "",
            Vector2 relativePos = default)
        {
            Font = TerraJS.Instance.FontManager[fontID, fontSize];

            Text = text;

            NewLineNum = newLineNum;

            TextHorizontalMiddle = textHorizontalMiddle;

            TextVerticalMiddle = textVerticalMiddle;

            SplitCharacter = splitCharacter;

            FontColor = fontColor == null ? Color.White : (Color)fontColor;

            var noColorText = Text.NoColored();

            if (size == default)
                size = Font.MeasureString(noColorText);

            _width = (int)size.X;

            _height = (int)size.Y;

            if (!string.IsNullOrEmpty(noColorText))
            {
                var texts = new List<string>();
                if (NewLineNum != int.MaxValue)
                    texts = noColorText.SplitWithCount(NewLineNum);
                if (SplitCharacter != null)
                    texts = noColorText.Split(SplitCharacter).ToList();

                if (_width == 0 || _height == 0)
                {
                    foreach (var txt in texts)
                        _width = Math.Max(_width, (int)Font.MeasureString(txt).X);
                    _height = (int)Font.MeasureString(texts[0]).Y * texts.Count;
                }
            }

            RelativePosition = relativePos == default ? new(0, 0) : relativePos;
        }
        internal int _width;

        internal int _height;

        public override int Width => _width;

        public override int Height => _height;

        public int NewLineNum;
        public string SplitCharacter;

        public bool TextHorizontalMiddle;
        public bool TextVerticalMiddle;

        public Color FontColor;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * Alpha);

            if (!string.IsNullOrEmpty(Text))
            {
                int x, y = 0;

                if (TextVerticalMiddle)
                    y = (Height - (int)Font.MeasureString(Text).Y) / 2;

                var size = Font.MeasureString(Text);

                x = (int)Position.X;

                var snippets = StringUtils.ParseMessage(Text, Color.White);

                if (TextHorizontalMiddle)
                    x += Width / 2 - (int)size.X / 2;

                spriteBatch.DrawColorCodedString(Font, snippets, new(x + 2, y + Position.Y), Color.Black * Alpha, 0f, Vector2.Zero, Vector2.One, out _, -1f, true);

                spriteBatch.DrawColorCodedString(Font, snippets, new(x, y + Position.Y + 2), Color.Black * Alpha, 0f, Vector2.Zero, Vector2.One, out _, -1f, true);

                spriteBatch.DrawColorCodedString(Font, snippets, new(x - 2, y + Position.Y), Color.Black * Alpha, 0f, Vector2.Zero, Vector2.One, out _, -1f, true);

                spriteBatch.DrawColorCodedString(Font, snippets, new(x, y + Position.Y - 2), Color.Black * Alpha, 0f, Vector2.Zero, Vector2.One, out _, -1f, true);

                spriteBatch.DrawColorCodedString(Font, snippets, new(x, y + Position.Y), FontColor * Alpha, 0f, Vector2.Zero, Vector2.One, out _, -1f);
                
                y += (int)size.Y;
            }

        }

        public void Resize()
        {
            var size = Font.MeasureString(Text);
            _width = (int)size.X;
            _height = (int)size.Y;
        }
    }
}
