using Jint.Native;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using TerraJS.Assets.AssetManagers;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Items;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API
{
    public abstract class ModTypeRegistry<T, K> : IRegistry<T> where T : ModType where K : ModTypeRegistry<T, K>
    {
        internal TypeBuilder _builder;

        internal TextureGetter _texture = new();

        internal static Type _contentType = typeof(T);
     
        internal static List<T> _tjsInstances = [];

        public bool IsEmpty = false;

        public Action<Type> AfterRegister;

        public abstract string Namespace { get; }

        public ModTypeRegistry(string name, string @namespace = "")
        {
            if (string.IsNullOrWhiteSpace(name) || @namespace.IsNullOrWhiteSpaceNotEmpty())
            {
                IsEmpty = true;

                return;
            }

            var modTypeName = $"TJSContents.{Namespace}.{(@namespace == "" ? "" : @namespace + ".")}{name}";

            _builder = GlobalAPI._mb.DefineType(modTypeName, TypeAttributes.Public, typeof(T));
        }

        public virtual void Register() => Register(TerraJS.Instance);

        [HideToJS]
        public abstract void Register(Mod mod);

        public K FromJS(ExpandoObject obj)
        {
            if (this is not K)
                return default;

            foreach (var item in obj)
            {
                if (item.Value is not Func<JsValue, JsValue[], JsValue> @delegate)
                    continue;

                var method = typeof(TJSItem).GetMethod(item.Key);

                if (method == null)
                    continue;

                RegistryUtils.OverrideJS(this, item.Key, @delegate);
            }

            return this as K;
        }
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
