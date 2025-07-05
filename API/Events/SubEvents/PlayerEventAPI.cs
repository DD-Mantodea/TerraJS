using System;
using Microsoft.Xna.Framework;
using TerraJS.API.Events.Ref;
using TerraJS.Contents.Attributes;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraJS.API.Events.SubEvents
{
    public class PlayerEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<ModPlayer> PostUpdateEvent;

        [HideToJS]
        public Action<ModPlayer, RefBox<FishingAttempt>> ModifyFishingAttempt;

        [HideToJS]
        public Action<ModPlayer, FishingAttempt, RefBox<int>, RefBox<int>, RefBox<AdvancedPopupRequest>, RefBox<Vector2>> CatchFishEvent;

        [EventInfo("modPlayer")]
        public void PostUpdate(Action<ModPlayer> @delegate) => PostUpdateEvent += @delegate;

        [EventInfo("modPlayer", "fishingAttempt")]
        public void PostUpdate(Action<ModPlayer, RefBox<FishingAttempt>> @delegate) => ModifyFishingAttempt += @delegate;

        [EventInfo("modPlayer", "fishingAttempt", "itemDrop", "npcSpawn", "sonar", "sonarPosition")]
        public void CatchFish(Action<ModPlayer, FishingAttempt, RefBox<int>, RefBox<int>, RefBox<AdvancedPopupRequest>, RefBox<Vector2>> @delegate) => CatchFishEvent += @delegate;
    }
}
