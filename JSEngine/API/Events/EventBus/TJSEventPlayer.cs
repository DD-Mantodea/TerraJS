using Microsoft.Xna.Framework;
using TerraJS.JSEngine.API.Events.Ref;
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
            if (TJSEngine.GlobalAPI.Event.Player.ModifyFishingAttemptEvent is not { } handler)
                return;

            using var attemptRef = new RefValue<FishingAttempt>(attempt);

            handler(this, attemptRef);

            attempt = attemptRef.Value;
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (TJSEngine.GlobalAPI.Event.Player.CatchFishEvent is not { } handler)
                return;

            using var itemDropRef = new RefValue<int>(itemDrop);

            using var npcSpawnRef = new RefValue<int>(npcSpawn);

            using var sonarRef = new RefValue<AdvancedPopupRequest>(sonar);

            using var sonarPositionRef = new RefValue<Vector2>(sonarPosition);

            handler(this, attempt, itemDropRef, npcSpawnRef, sonarRef, sonarPositionRef);

            itemDrop = itemDropRef.Value;

            npcSpawn = npcSpawnRef.Value;

            sonar = sonarRef.Value;

            sonarPosition = sonarPositionRef.Value;
        }
    }
}
