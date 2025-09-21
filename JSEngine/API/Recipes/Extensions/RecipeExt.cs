using Newtonsoft.Json.Linq;
using TerraJS.JSEngine.API.Items.Extensions;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Exceptions;

namespace TerraJS.JSEngine.API.Recipes.Extensions
{
    public static class RecipeExt
    {
        extension(Recipe recipe)
        {
            public Recipe AddIngredient(int itemID, string nbt, int stack = 1)
            {
                var item = new Item(itemID) { stack = stack };

                item.SetNbt(JObject.Parse(nbt));

                recipe.requiredItem.Add(item);

                return recipe;
            }

            public Recipe AddIngredient(ModItem item, string nbt, int stack = 1) => recipe.AddIngredient(item.Type, nbt, stack);

            public Recipe AddIngredient(Mod mod, string itemName, string nbt, int stack = 1)
            {
                mod ??= recipe.Mod;

                if (!ModContent.TryFind(mod.Name, itemName, out ModItem item))
                    throw new RecipeException($"The item {itemName} does not exist in the mod {mod.Name}.\r\nIf you are trying to use a vanilla item, try removing the first argument.");

                return recipe.AddIngredient(item, nbt, stack);
            }
        }
    }
}
