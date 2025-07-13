using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.JSEngine;
using Terraria.ModLoader;

namespace TerraJS.API.Events.EventBus
{
    public class TJSEventSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            TJSEngine.GlobalAPI.Event.Recipe.PostAddRecipesEvent?.Invoke();
        }
    }
}
