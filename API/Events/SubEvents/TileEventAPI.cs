using System;
using TerraJS.API.Events.Ref;
using TerraJS.Contents.Attributes;
using Terraria;

namespace TerraJS.API.Events.SubEvents
{
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

        [EventInfo("x", "y", "type", "item")]
        public void PlaceTile(Action<int, int, int, Item> @delegate) => PlaceTileEvent += @delegate;

        [EventInfo("x", "y", "type")]
        public void CanPlaceTile(Func<int, int, int, bool> @delegate) => CanPlaceTileEvent += @delegate;

        [EventInfo("x", "y", "type", "fail", "effectOnly", "noItem")]
        public void BreakTile(Action<int, int, int, RefBox<bool>, RefBox<bool>, RefBox<bool>> @delegate)
            => BreakTileEvent += @delegate;

        [EventInfo("x", "y", "type", "blockDamaged")]
        public void CanBreakTile(Func<int, int, int, RefBox<bool>, bool> @delegate) => CanBreakTileEvent += @delegate;
    }
}
