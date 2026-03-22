using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Events.EventBus
{
    public class TJSEventSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            TJSEngine.GlobalAPI.Event.Recipe.PostAddRecipesEvent?.Invoke();
        }
    }
}
