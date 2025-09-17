using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.JSEngine;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    public class RecipeHook : ModSystem
    {
        public override void Load()
        {
            var method = typeof(Recipe).GetMethod("Register");

            MonoModHooks.Add(method, PreRegister);
        }

        public static Recipe PreRegister(Func<Recipe, Recipe> orig, Recipe recipe)
        {
            var cancel = TJSEngine.GlobalAPI.Event.Recipe.PreRegisterRecipeEvent?.Invoke(recipe) ?? false;

            if (!cancel)
                return orig(recipe);

            return recipe;
        }
    }
}
