using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace TerraJS.Managers
{
    public class TextureManager : AssetManager<Texture2D>
    {
        public Dictionary<string, Texture2D> Textures = [];

        public TerraJS Mod => TerraJS.Instance;

        public override void LoadOne(string dir, Dictionary<string, Texture2D> dictronary)
        {
            foreach (var file in Mod.GetFileNames().Where(name => name.StartsWith(dir)))
            {
            }
        }
    }
}
