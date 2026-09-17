using TerraJS.JSEngine.API.Events.Ref;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Events.EventBus.Global
{
    public class TJSEventGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int x, int y, int type, Item item)
        {
            TJSEngine.GlobalAPI.Event.Tile.PlaceTileEvent?.Invoke(x, y, type, item);
        }

        public override bool CanPlace(int x, int y, int type)
        {
            return TJSEngine.GlobalAPI.Event.Tile.CanPlaceTileEvent?.Invoke(x, y, type) ?? true;
        }

        public override void KillTile(int x, int y, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (TJSEngine.GlobalAPI.Event.Tile.BreakTileEvent is not { } handler)
                return;

            using var failRef = new RefValue<bool>(fail);

            using var effectOnlyRef = new RefValue<bool>(effectOnly);

            using var noItemRef = new RefValue<bool>(noItem);

            handler(x, y, type, failRef, effectOnlyRef, noItemRef);

            fail = failRef.Value;

            effectOnly = effectOnlyRef.Value;

            noItem = noItemRef.Value;
        }

        public override bool CanKillTile(int x, int y, int type, ref bool blockDamaged)
        {
            if (TJSEngine.GlobalAPI.Event.Tile.CanBreakTileEvent is not { } handler)
                return true;

            using var blockDamagedRef = new RefValue<bool>(blockDamaged);

            var result = handler(x, y, type, blockDamagedRef);

            blockDamaged = blockDamagedRef.Value;

            return result;
        }
    }
}
