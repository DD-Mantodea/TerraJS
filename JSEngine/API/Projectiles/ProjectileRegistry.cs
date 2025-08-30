using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TerraJS.Assets.Managers;
using TerraJS.JSEngine;
using TerraJS.JSEngine.API;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Projectiles
{
    public class ProjectileRegistry : ModTypeRegistry<TJSProjectile>
    {
        internal static Dictionary<string, int> _contentTypes = [];

        public override string Namespace => "Projectiles";

        public ProjectileRegistry(string name, string @namespace) : base(name, @namespace) { }

        public ProjectileRegistry Texture(string path)
        {
            if (IsEmpty) return this;

            _texture.TexturePath = path;

            _texture.TextureType = TextureType.Empty;

            _texture.ID = -1;

            return this;
        }

        public ProjectileRegistry Texture(TextureType type, int ID)
        {
            if (IsEmpty) return this;

            _texture.TexturePath = "";

            _texture.TextureType = type;

            _texture.ID = ID;

            return this;
        }

        public ProjectileRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (IsEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public override void Register(Mod mod)
        {
            if (IsEmpty) return;

            var projType = _builder.CreateType();

            var JSProj = Activator.CreateInstance(projType) as TJSProjectile;

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSProj, [new Projectile()]);

            mod.AddContent(JSProj);

            _contentTypes.Add(_builder.FullName, JSProj.Type);

            _tjsInstances.Add(JSProj);

            TJSEngine.GlobalAPI.Event.PostSetupContent(() =>
            {
                TextureAssets.Projectile[JSProj.Type] = _texture.Get() ?? TextureAssets.Projectile[JSProj.Type];
            });
        }
    }
}
