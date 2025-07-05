using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TerraJS.Assets.Managers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API
{
    public abstract class Registry<T>
    {
        internal TypeBuilder _builder;

        internal TextureGetter _texture = new();

        internal static Type _contentType = typeof(T);

        internal TerraJS TJSMod = TerraJS.Instance;

        internal static List<T> _tjsInstances = [];

        public Dictionary<string, Delegate> _delegates = [];

        public bool isEmpty = false;

        public abstract void Register();
    }

    public class TextureGetter
    {
        public int ID = -1;

        public TextureType TextureType = TextureType.Empty;

        public string TexturePath = "";

        public Asset<Texture2D> Get()
        {
            Asset<Texture2D> texture = null;

            if (TextureType != TextureType.Empty && ID != -1)
                TerraJS.TextureManager.TryGetVanillaTexture(TextureType, ID, out texture); 

            if(TexturePath != "")
                TerraJS.TextureManager.Textures.TryGetValue(TexturePath, out texture);
            
            return texture;
        }
    }
}
