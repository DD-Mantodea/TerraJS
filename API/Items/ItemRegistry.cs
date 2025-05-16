using Jint.Native;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
using TerraJS.Utils;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Items
{
    public class ItemRegistry : Registry<JSItem>
    {
        public static ItemRegistry Empty => new() { isEmpty = true };

        public ItemRegistry() { }

        public ItemRegistry(TypeBuilder builder)
        {
            _builder = builder;

            TranslationAPI.AddTranslation(GameCulture.DefaultCulture, $"Mods.{_builder.FullName}.DisplayName", _builder.Name);

            TranslationAPI.AddTranslation(GameCulture.DefaultCulture, $"Mods.{_builder.FullName}.Tooltip", "");
        }

        public ItemRegistry Texture(string path)
        {
            if (isEmpty) return this;

            _texturePath = path;
            return this;
        }

        public ItemRegistry SetDefaults(Delegate @delegate)
        {
            if (isEmpty) return this;

            _delegates["SetDefaults"] = @delegate;

            return this;
        }

        public ItemRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (isEmpty) return this;

            TranslationAPI.AddTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public ItemRegistry Tooltip(GameCulture.CultureName gameCulture, string str)
        {
            if (isEmpty) return this;

            TranslationAPI.AddTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.Tooltip", str);

            return this;
        }

        public override void Register()
        {
            if (isEmpty) return;

            var itemType = _builder.CreateType();

            var JSItem = Activator.CreateInstance(itemType) as JSItem;

            ItemAPI.ItemDelegates.Add(itemType.Name, _delegates);

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSItem, [new Item()]);

            TJSMod.AddContent(JSItem);

            ContentTypes.Add(_builder.FullName, JSItem.Type);

            if (_texturePath != "")
            {
                TerraJS.GlobalAPI.Event.OnEvent("PostSetupContent", () =>
                {
                    TextureAssets.Item[JSItem.Type] = TJSMod.Assets.CreateUntracked<Texture2D>(File.OpenRead(_texturePath), _texturePath);
                });
            }
        }
    }
}
