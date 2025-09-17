using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace TerraJS.Contents.UI.Components
{
    public class UIText : Component
    {
        public UIText(string fontID, string text = "", int fontSize = 20, Color? fontColor = null)
        {
            Font = TerraJS.FontManager[fontID, fontSize];

            Snippets = SnippetUtils.ParseMessage(text);

            FontColor = fontColor == null ? Color.White : (Color)fontColor;

            var size = SnippetUtils.GetSize(Snippets, Font).Add(4, 4);

            _width = (int)size.X;

            _height = (int)size.Y;
        }

        public UIText(TerraJSFont font, string text = "", Color? fontColor = null)
        {
            Font = font;

            Snippets = SnippetUtils.ParseMessage(text);

            FontColor = fontColor == null ? Color.White : (Color)fontColor;

            var size = SnippetUtils.GetSize(Snippets, Font).Add(4, 4);

            _width = (int)size.X;

            _height = (int)size.Y;
        }

        public UIText(string fontID, List<TextSnippet> snippets, int fontSize = 20, Color? fontColor = null)
        {
            Font = TerraJS.FontManager[fontID, fontSize];

            Snippets = snippets;

            FontColor = fontColor == null ? Color.White : (Color)fontColor;

            var size = SnippetUtils.GetSize(Snippets, Font).Add(4, 4);

            _width = (int)size.X;

            _height = (int)size.Y;
        }

        public UIText(TerraJSFont font, List<TextSnippet> snippets, Color? fontColor = null)
        {
            Font = font;

            Snippets = snippets;

            FontColor = fontColor == null ? Color.White : (Color)fontColor;

            var size = SnippetUtils.GetSize(Snippets, Font).Add(4, 4);

            _width = (int)size.X;

            _height = (int)size.Y;
        }

        public override int Width => _width;

        public override int Height => _height;

        public bool TextHorizontalMiddle = false;
        public bool TextVerticalMiddle = false;

        public Color FontColor;

        public List<TextSnippet> Snippets = [];

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * Alpha);

            int x, y = 0;

            if (TextVerticalMiddle)
                y = (Height - (int)Font.MeasureString(SnippetUtils.GetPlainText(Snippets)).Y) / 2;

            var size = Font.MeasureString(SnippetUtils.GetPlainText(Snippets));

            x = (int)Position.X;

            if (TextHorizontalMiddle)
                x += Width / 2 - (int)size.X / 2;

            spriteBatch.DrawSnippets(Font, Snippets, new(x, y + Position.Y));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Resize();
        }

        public void Resize()
        {
            var size = Font.MeasureString(Text);
            _width = (int)size.X;
            _height = (int)size.Y;
        }

        public void SetText(string text)
        {
            Snippets = SnippetUtils.ParseMessage(text);
        }
    }
}
