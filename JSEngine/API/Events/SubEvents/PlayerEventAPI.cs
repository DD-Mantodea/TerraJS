using System;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Attributes;
using TerraJS.JSEngine.API.Events.Ref;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Events.SubEvents
{
    public class PlayerEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<ModPlayer> PostUpdateEvent;

        [HideToJS]
        public Action<ModPlayer, RefValue<FishingAttempt>> ModifyFishingAttemptEvent;

        [HideToJS]
        public Action<ModPlayer, FishingAttempt, RefValue<int>, RefValue<int>, RefValue<AdvancedPopupRequest>, RefValue<Vector2>> CatchFishEvent;

        [EventInfo("modPlayer")]
        public void PostUpdate(Action<ModPlayer> @delegate) => PostUpdateEvent += @delegate;

        [EventInfo("modPlayer", "fishingAttempt")]
        public void ModifyFishingAttempt(Action<ModPlayer, RefValue<FishingAttempt>> @delegate) => ModifyFishingAttemptEvent += @delegate;

        [EventInfo("modPlayer", "fishingAttempt", "itemDrop", "npcSpawn", "sonar", "sonarPosition")]
        public void CatchFish(Action<ModPlayer, FishingAttempt, RefValue<int>, RefValue<int>, RefValue<AdvancedPopupRequest>, RefValue<Vector2>> @delegate) => CatchFishEvent += @delegate;
    }
}
