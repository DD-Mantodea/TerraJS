using FontStashSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerraJS.Assets.Managers
{
    public class FontManager : AssetManager<FontSystem>
    {
        public Dictionary<string, FontSystem> Fonts;

        public TerraJS Mod => TerraJS.Instance;

        public override void LoadOne(string dir, Dictionary<string, FontSystem> dictronary)
        {
            foreach (var file in Mod.GetFileNames().Where(name => name.StartsWith("Assets/" + dir)))
            {
                var font = new FontSystem();
                font.AddFont(Mod.GetFileBytes(file));
                dictronary.Add(Path.GetFileNameWithoutExtension(file), font);
            }
        }

        public SpriteFontBase this[string index, float size]
        {
            get => Fonts[index].GetFont(size);
        }
    }
}
