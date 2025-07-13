using TerraJS.Contents.Attributes;
using TerraJS.JSEngine;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events.EventBus.Global
{
    [HideToJS]
    public class TJSEventGlobalItem : GlobalItem
    {
        public override void AddRecipes()
        {
            TJSEngine.GlobalAPI.Event.Recipe.AddRecipesEvent?.Invoke();
        }

        public override void UpdateInventory(Item item, Player player)
        {
            TJSEngine.GlobalAPI.Event.Item.UpdateInventoryEvent?.Invoke(item, player);
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            TJSEngine.GlobalAPI.Event.Item.UpdateAccessoryEvent?.Invoke(item, player, hideVisual);
        }

        public override void SetDefaults(Item item)
        {
            TJSEngine.GlobalAPI.Event.Item.SetDefaultsEvent?.Invoke(item);
        }

        public override void RightClick(Item item, Player player)
        {
            TJSEngine.GlobalAPI.Event.Item.RightClickEvent?.Invoke(item, player);
        }

        public override bool CanRightClick(Item item)
        {
            return TJSEngine.GlobalAPI.Event.Item.CanRightClickEvent?.Invoke(item) ?? false;
        }

        public override bool ConsumeItem(Item item, Player player)
        {
            return TJSEngine.GlobalAPI.Event.Item.ConsumeItemEvent?.Invoke(item, player) ?? true;
        }

        public override bool? UseItem(Item item, Player player)
        {
            return TJSEngine.GlobalAPI.Event.Item.UseItemEvent?.Invoke(item, player) ?? null;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return TJSEngine.GlobalAPI.Event.Item.CanUseItemEvent?.Invoke(item, player) ?? true;
        }
    }
}
