using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
