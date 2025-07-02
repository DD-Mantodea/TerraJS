using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TerraJS.Attributes;
using TerraJS.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events
{
    public class BaseEventAPI : BaseAPI
    {
        internal override void Unload()
        {
            GetType().GetFields().Where(f => typeof(Delegate).IsAssignableFrom(f.FieldType))
                .ToList().ForEach(f => f.SetValue(this, null));
        }
    }

    public class EventAPI : BaseEventAPI
    {
        public ItemEventAPI Item = new();

        public TileEventAPI Tile = new();

        public RecipeEventAPI Recipe = new();

        [HideToJS]
        public Action ModLoadEvent;

        [HideToJS]
        public Action InGameReloadEvent;

        [HideToJS]
        public Action PostSetupContentEvent;

        [EventInfo([])]
        public void ModLoad(Action @delegate) => ModLoadEvent += @delegate;

        [EventInfo([])]
        public void InGameReload(Action @delegate) => InGameReloadEvent += @delegate;

        [EventInfo([])]
        public void PostSetupContent(Action @delegate) => PostSetupContentEvent += @delegate;

        internal override void Unload()
        {
            base.Unload();

            Item.Unload();
            Tile.Unload();
            Recipe.Unload();
        }
    }

    public class ItemEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<Item, Player> UpdateInventoryEvent;

        [HideToJS]
        public Action<Item> SetDefaultsEvent;

        [HideToJS]
        public Action<Item, Player> RightClickEvent;

        [HideToJS]
        public Func<Item, bool> CanRightClickEvent;

        [HideToJS]
        public Func<Item, Player, bool?> UseItemEvent;

        [HideToJS]
        public Func<Item, Player, bool> CanUseItemEvent;

        [HideToJS]
        public Func<Item, Player, bool> ConsumeItemEvent;

        [EventInfo(["item", "player"])]
        public void UpdateInventory(Action<Item, Player> @delegate) => UpdateInventoryEvent += @delegate;

        [EventInfo(["item"])]
        public void SetDefaults(Action<Item> @delegate) => SetDefaultsEvent += @delegate;

        [EventInfo(["item", "player"])]
        public void RightClick(Action<Item, Player> @delegate) => RightClickEvent += @delegate;

        [EventInfo(["item"])]
        public void CanRightClick(Func<Item, bool> @delegate) => CanRightClickEvent += @delegate;

        [EventInfo(["item", "player"])]
        public void UseItem(Func<Item, Player, bool?> @delegate) => UseItemEvent += @delegate;

        [EventInfo(["item", "player"])]
        public void CanUseItem(Func<Item, Player, bool> @delegate) => CanUseItemEvent += @delegate;

        [EventInfo(["item", "player"])]
        public void ConsumeItem(Func<Item, Player, bool> @delegate) => ConsumeItemEvent += @delegate;
    }

    public class TileEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<int, int, int, Item> PlaceTileEvent;

        [HideToJS]
        public Func<int, int, int, bool> CanPlaceTileEvent;

        [HideToJS]
        public Action<int, int, int, RefBox<bool>, RefBox<bool>, RefBox<bool>> BreakTileEvent;

        [HideToJS]
        public Func<int, int, int, RefBox<bool>, bool> CanBreakTileEvent;

        [EventInfo(["x", "y", "type", "item"])]
        public void PlaceTile(Action<int, int, int, Item> @delegate) => PlaceTileEvent += @delegate;

        [EventInfo(["x", "y", "type"])]
        public void CanPlaceTile(Func<int, int, int, bool> @delegate) => CanPlaceTileEvent += @delegate;

        [EventInfo(["x", "y", "type", "fail", "effectOnly", "noItem"])]
        public void BreakTile(Action<int, int, int, RefBox<bool>, RefBox<bool>, RefBox<bool>> @delegate)
            => BreakTileEvent += @delegate;

        [EventInfo(["x", "y", "type", "blockDamaged"])]
        public void CanBreakTile(Func<int, int, int, RefBox<bool>, bool> @delegate) => CanBreakTileEvent += @delegate;
    }

    public class RecipeEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action AddRecipesEvent;

        [HideToJS]
        public Action PostAddRecipesEvent;

        [EventInfo([])]
        public void AddRecipes(Action @delegate) => AddRecipesEvent += @delegate;

        [EventInfo([])]
        public void PostAddRecipes(Action @delegate) => PostAddRecipesEvent += @delegate;
    }
}
