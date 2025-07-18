﻿using System;
using TerraJS.API.Events.Ref;
using TerraJS.Contents.Attributes;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events.SubEvents
{
    public class NPCEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<NPC, NPCLoot> ModifyNPCLootEvent;

        [HideToJS]
        public Func<NPC, bool> PreKillEvent;

        [HideToJS]
        public Action<NPCShop> ModifyShopEvent;

        [HideToJS]
        public Action<NPC, string, RefBoxArray<Item>> ModifyActiveShopEvent;

        [EventInfo("npc", "npcLoot")]
        public void ModifyNPCLoot(Action<NPC, NPCLoot> @delegate) => ModifyNPCLootEvent += @delegate;

        [EventInfo("npc")]
        public void PreKill(Func<NPC, bool> @delegate) => PreKillEvent += @delegate;

        [EventInfo("shop")]
        public void ModifyShop(Action<NPCShop> @delegate) => ModifyShopEvent += @delegate;

        [EventInfo("npc", "shopName", "items")]
        public void ModifyActiveShop(Action<NPC, string, RefBoxArray<Item>> @delegate) => ModifyActiveShopEvent += @delegate;
    }
}
