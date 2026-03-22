using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Items
{
    [Autoload(false)]
    public abstract class TJSItem : ModItem
    {
        public override string Texture => "TerraJS/Assets/Textures/NULL";
    }
}
