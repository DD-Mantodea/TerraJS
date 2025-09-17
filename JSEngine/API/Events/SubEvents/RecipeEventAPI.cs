using System;
using TerraJS.Contents.Attributes;
using Terraria;

namespace TerraJS.API.Events.SubEvents
{
    public class RecipeEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action AddRecipesEvent;

        [HideToJS]
        public Action PostAddRecipesEvent;

        [HideToJS]
        public Func<Recipe, bool> PreRegisterRecipeEvent;

        [EventInfo]
        public void AddRecipes(Action @delegate) => AddRecipesEvent += @delegate;

        [EventInfo]
        public void PostAddRecipes(Action @delegate) => PostAddRecipesEvent += @delegate;

        [EventInfo("recipe")]
        public void PreRegisterRecipe(Func<Recipe, bool> @delegate) => PreRegisterRecipeEvent += @delegate;
    }
}
