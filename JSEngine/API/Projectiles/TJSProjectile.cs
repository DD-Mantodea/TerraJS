using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Projectiles
{
    [Autoload(false)]
    public abstract class TJSProjectile : ModProjectile
    {
        public override string Texture => "TerraJS/Assets/Textures/NULL";
    }
}
