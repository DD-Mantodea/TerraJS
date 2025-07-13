using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TerraJS.Assets.Managers;
using TerraJS.JSEngine;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace TerraJS.API.Projectiles
{
    public class ProjectileRegistry : Registry<TJSProjectile>
    {
        internal static Dictionary<string, int> _contentTypes = [];

        public static ProjectileRegistry Empty => new() { isEmpty = true };

        public ProjectileRegistry() { }

        public ProjectileRegistry(TypeBuilder builder) 
        {
            _builder = builder;
        }

        public ProjectileRegistry Texture(string path)
        {
            if (isEmpty) return this;

            _texture.TexturePath = path;

            _texture.TextureType = TextureType.Empty;

            _texture.ID = -1;

            return this;
        }

        public ProjectileRegistry Texture(TextureType type, int ID)
        {
            if (isEmpty) return this;

            _texture.TexturePath = "";

            _texture.TextureType = type;

            _texture.ID = ID;

            return this;
        }

        public ProjectileRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (isEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public override void Register()
        {
            if (isEmpty) return;

            var projType = _builder.CreateType();

            var JSProj = Activator.CreateInstance(projType) as TJSProjectile;

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSProj, [new Projectile()]);

            TJSMod.AddContent(JSProj);

            _contentTypes.Add(_builder.FullName, JSProj.Type);

            _tjsInstances.Add(JSProj);

            TJSEngine.GlobalAPI.Event.PostSetupContent(() =>
            {
                TextureAssets.Projectile[JSProj.Type] = _texture.Get() ?? TextureAssets.Projectile[JSProj.Type];
            });
        }
    }
}
