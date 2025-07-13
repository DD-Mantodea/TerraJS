using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.Utils;
using Terraria;

namespace TerraJS.API.Tiles
{
    public class TileAPI : BaseAPI
    {
        public void KillTile(int x, int y, dynamic options = default)
        {
            var fail = OptionUtils.GetOption<bool>(options, "fail", false);

            var effectOnly = OptionUtils.GetOption<bool>(options, "effectOnly", false);

            var noItem = OptionUtils.GetOption<bool>(options, "noItem", false);

            WorldGen.KillTile(x, y, fail, effectOnly, noItem);
        }

        public Tile MouseTile
        {
            get
            {
                var mouseTileCoords = Main.MouseWorld.ToTileCoordinates();

                return Main.tile[mouseTileCoords.X, mouseTileCoords.Y];
            }
        }

        internal override void Unload()
        {
        }
    }
}
