using FontStashSharp;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.GameContent;

namespace TerraJS.Assets.Managers
{
    public class FontManager : AssetManager<FontSystem>
    {
        public Dictionary<string, FontSystem> Fonts;

        public static TerraJS Mod => TerraJS.Instance;

        public override void Load()
        {
            base.Load();

            if (!TerraJS.IsLoading)
                return;
        }

        public override void LoadOne(string dir, Dictionary<string, FontSystem> dictronary)
        {
            foreach (var file in Mod.GetFileNames().Where(name => name.StartsWith("Assets/" + dir)))
            {
                if (file.EndsWith(".ttf"))
                {
                    var font = new FontSystem();

                    font.AddFont(Mod.GetFileBytes(file));

                    dictronary.Add(Path.GetFileNameWithoutExtension(file), font);
                }
            }
        }

        public static StaticSpriteFont FromVanilla(string name)
        {
            var font = name.Replace("Vanilla:", "") switch
            {
                "MouseText" => FontAssets.MouseText.Value,
                _ => null
            };

            if (font == null)
                return null;

            var result = new StaticSpriteFont(font.LineSpacing, font.LineSpacing);

            var characters = font.SpriteCharacters;

            foreach (var c in characters.Keys.Order())
            {
                var data = characters[c];

                var glyph = new FontGlyph
                {
                    Id = c,
                    Codepoint = c,
                    TextureOffset = new Point(data.Glyph.X, data.Glyph.Y),
                    Size = new Point(data.Glyph.Width, data.Glyph.Height),
                    Texture = data.Texture
                };

                result.Glyphs[c] = glyph;

                foreach (var c2 in characters.Keys.Order())
                {
                    result.SetGlyphKernAdvance(c2, c, (int)data.Kerning.X);

                    result.SetGlyphKernAdvance(c, c2, (int)data.Kerning.Z);
                }
            }

            return result;
        }

        public SpriteFontBase this[string index, float size]
        {
            get {
                return Fonts[index].GetFont(size);
            }
        }
    }
}
