using System.IO;
using Terraria;

namespace TerraJS
{
    public class Pathes
    {
        public static string ModsPath => Path.Combine(Main.SavePath, "Mods");

        public static string TerraJSPath => Path.Combine(ModsPath, "TerraJS");

        public static string ModPacksPath => Path.Combine(ModsPath, "TjsModPacks");    }
}
