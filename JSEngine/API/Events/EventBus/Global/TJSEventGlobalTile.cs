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
            TJSEngine.GlobalAPI.Event.Tile.BreakTileEvent?.Invoke(x, y, type, new(fail), new(effectOnly), new(noItem));
        }

        public override bool CanKillTile(int x, int y, int type, ref bool blockDamaged)
        {
            return TJSEngine.GlobalAPI.Event.Tile.CanBreakTileEvent?.Invoke(x, y, type, new(blockDamaged)) ?? true;
        }
    }
}
