using System.Collections.Generic;
using System.Text.RegularExpressions;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;

namespace TerraJS.Contents.UI.Chat.TextSnippnts
{
    public class ColorTextSnippet(string text, string originalText, Color color) : TextSnippet(originalText)
    {
        public ColorTextSnippet() : this("", "", default) { }

        public string Text = text;

        public Color Color = color;

        public override string[] Identifiers => ["c", "color"];

        public Regex Regex = new("(c|color)/([0-9A-F]{6}):(.*)");

        public override Vector2 GetSize(SpriteFontBase font)
        {
            return font.MeasureString(Text);
        }

        public override List<TextSnippet> SplitByWidth(SpriteFontBase font, int width)
        {
            var result = new List<TextSnippet>();

            for (var i = 0; i < Text.Length; i++)
            {
                var subStr = Text[0..i];

                var strWidth = (int)font.MeasureString(subStr).X;

                if (strWidth > width)
                {
                    var text_1 = Text[0..(i - 1)];

                    var text_2 = Text[(i - 1)..^0];

                    result.Add(new ColorTextSnippet(text_1, $"[c/{Color.ToHexString()}:{text_1}]", Color));

                    result.Add(new ColorTextSnippet(text_2, $"[c/{Color.ToHexString()}:{text_2}]", Color));

                    break;
                }
            }

            return result;
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFontBase font, ref Vector2 position)
        {
            spriteBatch.DrawBorderedString(font, Text, position, Color, Color.Black, 2);

            position = position.Add(font.MeasureString(Text).X, 0);
        }

        public override TextSnippet Parse(string str)
        {
            var match = Regex.Match(str);

            if (match.Success)
            {
                var color = ColorUtils.FromHexString(match.Groups[2].Value);

                var text = match.Groups[3].Value;

                return new ColorTextSnippet(text, $"[{str}]", color);
            }

            return null;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
