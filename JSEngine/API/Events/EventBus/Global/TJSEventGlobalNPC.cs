using TerraJS.Contents.Attributes;
using TerraJS.JSEngine;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events.EventBus.Global
{
    public unsafe class TJSEventGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            TJSEngine.GlobalAPI.Event.NPC.ModifyNPCLootEvent?.Invoke(npc, npcLoot);
        }

        public override bool PreKill(NPC npc)
        {
            return TJSEngine.GlobalAPI.Event.NPC.PreKillEvent?.Invoke(npc) ?? true;
        }

        public override void ModifyShop(NPCShop shop)
        {
            TJSEngine.GlobalAPI.Event.NPC.ModifyShopEvent?.Invoke(shop);
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            fixed (Item* pItems = items)
            {
                TJSEngine.GlobalAPI.Event.NPC.ModifyActiveShopEvent?.Invoke(npc, shopName, new(pItems, items.Length));
            }
        }

        public override bool PreAI(NPC npc)
        {
            return TJSEngine.GlobalAPI.Event.NPC.PreAIEvent?.Invoke(npc) ?? true;
        }

        public override void AI(NPC npc)
        {
            TJSEngine.GlobalAPI.Event.NPC.AIEvent?.Invoke(npc);
        }

        public override void PostAI(NPC npc)
        {
            TJSEngine.GlobalAPI.Event.NPC.PostAIEvent?.Invoke(npc);
        }
    }
}
