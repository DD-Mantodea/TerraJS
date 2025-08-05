using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TerraJS.Assets.Managers;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Items
{
    public class ItemRegistry : Registry<TJSItem>
    {
        public static ItemRegistry Empty => new() { IsEmpty = true };

        internal static Dictionary<string, int> _contentTypes = [];

        private Dictionary<EquipType, TextureGetter> _equipTexture = [];

        public ItemRegistry() { }

        public ItemRegistry(TypeBuilder builder)
        {
            _builder = builder;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, $"Mods.{_builder.FullName}.DisplayName", _builder.Name);

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, $"Mods.{_builder.FullName}.Tooltip", "");
        }

        public ItemRegistry Texture(string path)
        {
            if (IsEmpty) return this;

            _texture.TexturePath = path;

            _texture.TextureType = TextureType.Empty;

            _texture.ID = -1;

            return this;
        }

        public ItemRegistry Texture(TextureType type, int ID)
        {
            if (IsEmpty) return this;

            _texture.TexturePath = "";

            _texture.TextureType = type;

            _texture.ID = ID;

            return this;
        }

        public ItemRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (IsEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public ItemRegistry Tooltip(GameCulture.CultureName gameCulture, string str)
        {
            if (IsEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.Tooltip", str);

            return this;
        }

        public ItemRegistry Equipment(EquipType[] type)
        {
            if (IsEmpty) return this;

            var attrBuilder = new CustomAttributeBuilder(typeof(AutoloadEquip).GetConstructors()[0], [ type ]);

            _builder.SetCustomAttribute(attrBuilder);

            return this;
        }

        public ItemRegistry EquipmentTexture(string path, EquipType type)
        {
            if (IsEmpty) return this;

            _equipTexture.AddOrSet(type, new() { TexturePath = path });

            return this;
        }

        public override void Register()
        {
            if (IsEmpty) return;

            var itemType = _builder.CreateType();

            var JSItem = Activator.CreateInstance(itemType) as TJSItem;

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSItem, [new Item()]);

            TJSMod.AddContent(JSItem);

            _contentTypes.Add(_builder.FullName, JSItem.Type);

            _tjsInstances.Add(JSItem);

            TJSEngine.GlobalAPI.Event.PostSetupContent(() =>
            {
                TextureAssets.Item[JSItem.Type] = _texture.Get() ?? TextureAssets.Item[JSItem.Type];

                var types = JSItem.GetType().GetCustomAttribute<AutoloadEquip>()?.equipTypes;

                if (types == null)
                    return;

                foreach (var type in types)
                {
                    if (_equipTexture.TryGetValue(type, out var getter))
                    {
                        var id = EquipLoader.GetEquipSlot(TJSMod, _builder.Name, type);

                        var array = GetTextureArray(type);

                        array[id] = getter.Get() ?? array[id];
                    }
                }
            });
        }

        private static Asset<Texture2D>[] GetTextureArray(EquipType type)
        {
            return typeof(EquipLoader).GetMethod("GetTextureArray", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, [ type ]) as Asset<Texture2D>[];
        }
    }
}
