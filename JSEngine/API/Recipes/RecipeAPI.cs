using System;
using System.Collections.Generic;
using Terraria;

namespace TerraJS.JSEngine.API.Recipes
{
    public class RecipeAPI : BaseAPI
    {
        public Recipe CreateRecipe(int item, int amount = 1) => Recipe.Create(item, amount);
    
        public Recipe FindFirstRecipe(Func<Recipe, bool> predicate)
        {
            foreach(var recipe in Main.recipe)
            {
                if (predicate(recipe))
                    return recipe;
            }

            return null;
        }

        private IEnumerable<Recipe> EnumerableRecipes(Func<Recipe, bool> predicate)
        {
            foreach (var recipe in Main.recipe)
            {
                if (predicate(recipe))
                    yield return recipe;
            }
        }

        public Recipe[] FindAllRecipe(Func<Recipe, bool> predicate = null)
        {
            if (predicate == null)
                return Main.recipe;

            return [.. EnumerableRecipes(predicate)];
        }

        internal override void Unload()
        {
        }
    }
}
