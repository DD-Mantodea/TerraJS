using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.API.Items;
using TerraJS.Assets.Managers;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

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

        public ProjectileRegistry SetDefaults(Delegate @delegate)
        {
            if (isEmpty) return this;

            _delegates["SetDefaults"] = @delegate;

            return this;
        }

        public ProjectileRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (isEmpty) return this;

            TranslationAPI.AddTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public override void Register()
        {
            if (isEmpty) return;

            var projType = _builder.CreateType();

            var JSProj = Activator.CreateInstance(projType) as TJSProjectile;

            ProjectileAPI.ProjectileDelegates.Add(projType.Name, _delegates);

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSProj, [new Projectile()]);

            TJSMod.AddContent(JSProj);

            _contentTypes.Add(_builder.FullName, JSProj.Type);

            _tjsInstances.Add(JSProj);

            TerraJS.GlobalAPI.Event.PostSetupContent(() =>
            {
                TextureAssets.Projectile[JSProj.Type] = _texture.Get() ?? TextureAssets.Projectile[JSProj.Type];
            });
        }
    }
}
