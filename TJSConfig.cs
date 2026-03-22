using Terraria.ModLoader.Config;

namespace TerraJS
{
    public class TJSConfig : ModConfig
    {
        public override void OnLoaded()
        {
            Instance = this;
        }

        public static TJSConfig Instance { get; set; } = null!;

        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
}
