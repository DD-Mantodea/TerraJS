using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraJS
{
    public class Pathes
    {
        public static string ModsPath => Path.Combine(Main.SavePath, "Mods");

        public static string TerraJSPath => Path.Combine(ModsPath, "TerraJS");

        public static string ModPacksPath => Path.Combine(ModsPath, "TjsModPacks");    }
}
