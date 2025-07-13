using System;
using TerraJS.Contents.Attributes;

namespace TerraJS.API.Events.SubEvents
{
    public class RecipeEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action AddRecipesEvent;

        [HideToJS]
        public Action PostAddRecipesEvent;

        [EventInfo]
        public void AddRecipes(Action @delegate) => AddRecipesEvent += @delegate;

        [EventInfo]
        public void PostAddRecipes(Action @delegate) => PostAddRecipesEvent += @delegate;
    }
}
