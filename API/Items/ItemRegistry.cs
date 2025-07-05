using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TerraJS.Assets.Managers;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace TerraJS.API.Items
{
    public class ItemRegistry : Registry<TJSItem>
    {
        internal static Dictionary<string, int> _contentTypes = [];

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

            _texture.TexturePath = path;

            return this;
        }

        public ItemRegistry Texture(TextureType type, int ID)
        {
            if (isEmpty) return this;

            _texture.TextureType = type;

            _texture.ID = ID;

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

            var JSItem = Activator.CreateInstance(itemType) as TJSItem;

            ItemAPI.ItemDelegates.Add(itemType.FullName, _delegates);

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSItem, [new Item()]);

            TJSMod.AddContent(JSItem);

            _contentTypes.Add(_builder.FullName, JSItem.Type);

            _tjsInstances.Add(JSItem);

            TerraJS.GlobalAPI.Event.PostSetupContent(() =>
            {
                TextureAssets.Item[JSItem.Type] = _texture.Get() ?? TextureAssets.Item[JSItem.Type];
            });
        }
    }
}
