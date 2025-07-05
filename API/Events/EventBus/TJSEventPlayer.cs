using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraJS.API.Events.EventBus
{
    public unsafe class TJSEventPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            TerraJS.GlobalAPI.Event.Player.PostUpdateEvent?.Invoke(this);
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            fixed (FishingAttempt* pFishingAttempt = &attempt)
            {
                TerraJS.GlobalAPI.Event.Player.ModifyFishingAttempt?.Invoke(this, new(pFishingAttempt));
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            fixed (int* pItemDrop = &itemDrop, pNpcSpawn = &npcSpawn)
            {
                fixed (AdvancedPopupRequest* pSonar = &sonar)
                {
                    fixed (Vector2* pSonarPosition = &sonarPosition)
                    {
                        TerraJS.GlobalAPI.Event.Player.CatchFishEvent.Invoke(this, attempt, new(pItemDrop), new(pNpcSpawn), new(pSonar), new(pSonarPosition));
                    }
                }
            }
        }
    }
}
