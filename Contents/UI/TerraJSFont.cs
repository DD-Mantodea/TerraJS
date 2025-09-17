using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using TerraJS.Contents.Extensions;

namespace TerraJS.Contents.UI
{
    public class TerraJSFont
    {
        public FontStashSharp.SpriteFontBase FontStashFont = null;

        public ReLogic.Graphics.DynamicSpriteFont RelogicFont = null;

        public TerraJSFont(FontStashSharp.SpriteFontBase font)
        {
            FontStashFont = font;
        }

        public TerraJSFont(DynamicSpriteFont font)
        {
            RelogicFont = font;
        }

        public int LineHeight => FontStashFont?.LineHeight ?? RelogicFont.LineSpacing;

        public float FontSize => FontStashFont?.FontSize ?? RelogicFont.LineSpacing;

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 postion, Color color, float characterSpacing)
        {
            if (FontStashFont == null && RelogicFont == null)
                return;

            if (FontStashFont != null)
                FontStashFont.DrawText(spriteBatch, text, postion, color, characterSpacing: characterSpacing);
            else
                spriteBatch.DrawString(RelogicFont, text, postion.Add(0, 4), color);
        }

        public Vector2 MeasureString(string text, float characterSpacing = 0)
        {
            if (FontStashFont == null && RelogicFont == null)
                return Vector2.Zero;

            if (FontStashFont != null)
                return FontStashFont.MeasureString(text) + new Vector2(characterSpacing * (text.Length - 1), 0);
            else
                return RelogicFont.MeasureString(text);
        }
    }
}
