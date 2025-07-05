using TerraJS.Contents.Attributes;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events.EventBus.Global
{
    [HideToJS]
    public unsafe class TJSEventGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            TerraJS.GlobalAPI.Event.NPC.ModifyNPCLootEvent?.Invoke(npc, npcLoot);
        }

        public override bool PreKill(NPC npc)
        {
            return TerraJS.GlobalAPI.Event.NPC.PreKillEvent?.Invoke(npc) ?? true;
        }

        public override void ModifyShop(NPCShop shop)
        {
            TerraJS.GlobalAPI.Event.NPC.ModifyShopEvent?.Invoke(shop);
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            fixed (Item* pItems = items)
            {
                TerraJS.GlobalAPI.Event.NPC.ModifyActiveShopEvent?.Invoke(npc, shopName, new(pItems, items.Length));
            }
        }
    }
}
