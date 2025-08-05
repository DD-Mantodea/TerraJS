using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Native;
using TerraJS.API.Items;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Projectiles
{
    [Autoload(false)]
    public abstract class TJSProjectile : ModProjectile
    {
        public override string Texture => "TerraJS/Assets/Textures/NULL";
    }
}
