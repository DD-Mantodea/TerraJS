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
using Terraria;
using Terraria.GameContent;

namespace TerraJS.API.Projectiles
{
    public class ProjectileRegistry : Registry<JSProjectile>
    {
        public static ProjectileRegistry Empty => new() { isEmpty = true };

        public ProjectileRegistry() { }

        public ProjectileRegistry(TypeBuilder builder) 
        {
            _builder = builder;
        }

        public ProjectileRegistry Texture(string path)
        {
            if (isEmpty) return this;

            _texturePath = path;
            return this;
        }

        public ProjectileRegistry SetDefaults(Delegate @delegate)
        {
            if (isEmpty) return this;

            _delegates["SetDefaults"] = @delegate;

            return this;
        }

        public override void Register()
        {
            if (isEmpty) return;

            var projType = _builder.CreateType();

            var JSProj = Activator.CreateInstance(projType) as JSProjectile;

            ProjectileAPI.ProjectileDelegates.Add(projType.Name, _delegates);

            var entity = _contentType.GetProperty("Entity", BindingFlags.Public | BindingFlags.Instance);

            entity.GetSetMethod(true).Invoke(JSProj, [new Projectile()]);

            TJSMod.AddContent(JSProj);

            ContentTypes.Add(_builder.FullName, JSProj.Type);

            if(_texturePath != null)
            {
                TerraJS.GlobalAPI.Event.OnEvent("PostSetupContent", () =>
                {
                    TextureAssets.Item[JSProj.Type] = TJSMod.Assets.CreateUntracked<Texture2D>(File.OpenRead(_texturePath), _texturePath);
                });
            }
        }
    }
}
