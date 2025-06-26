using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraJS.API.Players
{
    public class PlayerAPI : BaseAPI
    {
        public Item[] GetPlayerAccessories(Player player) => player.armor[3..9];

        internal override void Reload()
        {
        }
    }
}
