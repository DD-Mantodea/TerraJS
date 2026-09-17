using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Events.EventBus
{
    public unsafe class TJSEventPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            TJSEngine.GlobalAPI.Event.Player.PostUpdateEvent?.Invoke(this);
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            TJSEngine.GlobalAPI.Event.Player.ModifyFishingAttempt?.Invoke(this, new(attempt));
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            TJSEngine.GlobalAPI.Event.Player.CatchFishEvent.Invoke(this, attempt, new(itemDrop), new(npcSpawn), new(sonar), new(sonarPosition));
        }
    }
}
