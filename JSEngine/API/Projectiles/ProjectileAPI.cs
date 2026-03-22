using System;
using System.Linq;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Projectiles
{
    public class ProjectileAPI : BaseAPI
    {
        public ProjectileRegistry CreateProjectileRegistry(string name, string @namespace = "") => new(name, @namespace);

        public int GetModProjectile(string modName, string projName)
        {
            if (modName == "TerraJS") return GetTJSProjectile(projName);

            if (ModLoader.TryGetMod(modName, out var mod))
            {
                var type = mod.GetType().Assembly.GetTypes().First(t => t.Name == projName);

                if (type == null) return -1;

                return GetModProjectile(type);
            }

            return -1;
        }

        public int GetModProjectile(Type type)
        {
            if (!type.IsSubclassOf(typeof(ModProjectile))) return -1;

            var projTypeMethod = typeof(ModContent).GetMethod("ProjectileType");

            return (int)projTypeMethod.MakeGenericMethod(type).Invoke(null, []);
        }

        public int GetTJSProjectile(string fullName)
        {
            int projType = -1;

            if (!ProjectileRegistry._contentTypes.TryGetValue($"TJSContents.Projectiles.{fullName}", out projType)) projType = -1;

            return projType;
        }

        internal override void Unload()
        {
        }
    }
}
