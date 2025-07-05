using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System.IO;
using Jint.Native;
using Jint;
using Terraria.DataStructures;

namespace TerraJS.API.Items
{
    [Autoload(false)]
    public class TJSItem : ModItem
    {
        public override string Texture => "TerraJS/Assets/Textures/NULL";
    }
}
