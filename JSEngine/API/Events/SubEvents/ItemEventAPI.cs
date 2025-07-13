using System;
using TerraJS.Contents.Attributes;
using Terraria;

namespace TerraJS.API.Events.SubEvents
{
    public class ItemEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<Item, Player> UpdateInventoryEvent;

        [HideToJS]
        public Action<Item, Player, bool> UpdateAccessoryEvent;

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

        [EventInfo("item", "player")]
        public void UpdateInventory(Action<Item, Player> @delegate) => UpdateInventoryEvent += @delegate;

        [EventInfo("item", "player", "hideVisual")]
        public void UpdateAccessory(Action<Item, Player, bool> @delegate) => UpdateAccessoryEvent += @delegate;

        [EventInfo("item")]
        public void SetDefaults(Action<Item> @delegate) => SetDefaultsEvent += @delegate;

        [EventInfo("item", "player")]
        public void RightClick(Action<Item, Player> @delegate) => RightClickEvent += @delegate;

        [EventInfo("item")]
        public void CanRightClick(Func<Item, bool> @delegate) => CanRightClickEvent += @delegate;

        [EventInfo("item", "player")]
        public void UseItem(Func<Item, Player, bool?> @delegate) => UseItemEvent += @delegate;

        [EventInfo("item", "player")]
        public void CanUseItem(Func<Item, Player, bool> @delegate) => CanUseItemEvent += @delegate;

        [EventInfo("item", "player")]
        public void ConsumeItem(Func<Item, Player, bool> @delegate) => ConsumeItemEvent += @delegate;
    }
}
