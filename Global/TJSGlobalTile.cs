using TerraJS.API.Events;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Global
{
    public unsafe class TJSGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int x, int y, int type, Item item)
        {
            TerraJS.GlobalAPI.Event.Tile.PlaceTileEvent?.Invoke(x, y, type, item);
        }

        public override bool CanPlace(int x, int y, int type)
        {
            return TerraJS.GlobalAPI.Event.Tile.CanPlaceTileEvent?.Invoke(x, y, type) ?? true;
        }

        public override void KillTile(int x, int y, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            fixed (bool* pFail = &fail, pEffectOnly = &effectOnly, pNoItem = &noItem)
            {
                TerraJS.GlobalAPI.Event.Tile.BreakTileEvent?.Invoke(x, y, type, new RefBox<bool>(pFail), new RefBox<bool>(pEffectOnly), new RefBox<bool>(pNoItem));
            }
        }

        public override bool CanKillTile(int x, int y, int type, ref bool blockDamaged)
        {
            fixed (bool* pBlockDamaged = &blockDamaged)
            {
                return TerraJS.GlobalAPI.Event.Tile.CanBreakTileEvent?.Invoke(x, y, type, new RefBox<bool>(pBlockDamaged)) ?? true;
            }
        }
    }
}
