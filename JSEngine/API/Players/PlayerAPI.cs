using Terraria;

namespace TerraJS.JSEngine.API.Players
{
    public class PlayerAPI : BaseAPI
    {
        public Item[] GetPlayerAccessories(Player player) => player.armor[3..9];

        internal override void Unload()
        {
        }
    }
}
