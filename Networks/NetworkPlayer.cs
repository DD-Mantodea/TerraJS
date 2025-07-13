using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Networks
{
    public class NetworkPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                CustomModCheckPacket.Get(ModCheckType.Script).Send();

                CustomModCheckPacket.Get(ModCheckType.Texture).Send();
            }
        }
    }
}
