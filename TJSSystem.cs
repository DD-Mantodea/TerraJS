using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TerraJS
{
    public class TJSSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            TerraJS.GlobalAPI.Event.InvokeEvent("PostAddRecipes");
        }
    }
}
