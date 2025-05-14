using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraJS.API.Recipes
{
    public class RecipeAPI
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

        public Recipe[] FindAllRecipe(Func<Recipe, bool> predicate)
        {
            return EnumerableRecipes(predicate).ToArray();
        }
    }
}
