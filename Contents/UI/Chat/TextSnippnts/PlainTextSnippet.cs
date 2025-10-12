using System.Collections.Generic;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Extensions;

namespace TerraJS.Contents.UI.Chat.TextSnippnts
{
    public class PlainTextSnippet(string text) : TextSnippet(text)
    {
        public PlainTextSnippet() : this("") { }

        public override string[] Identifiers => [];

        public override List<TextSnippet> SplitByWidth(SpriteFontBase font, int width)
        {
            var result = new List<TextSnippet>();

            for (var i = 0; i < OriginalText.Length; i++)
            {
                var subStr = OriginalText[0..i];

                var strWidth = (int)font.MeasureString(subStr).X;

                if (strWidth > width)
                {
                    var text_1 = OriginalText[0..(i - 1)];

                    var text_2 = OriginalText[(i - 1)..^0];

                    result.Add(new PlainTextSnippet(text_1));

                    result.Add(new PlainTextSnippet(text_2));

                    break;
                }
            }

            return result;
        }

        public override Vector2 GetSize(SpriteFontBase font)
        {
            return font.MeasureString(OriginalText);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFontBase font, ref Vector2 position)
        {
            spriteBatch.DrawBorderedString(font, OriginalText, position, Color.White, Color.Black, 2);

            position = position.Add(font.MeasureString(OriginalText).X, 0);
        }

        public override TextSnippet Parse(string str)
        {
            return new PlainTextSnippet(str);
        }

        public override string ToString()
        {
            return OriginalText;
        }
    }
}
