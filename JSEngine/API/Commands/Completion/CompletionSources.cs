using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TerraJS.JSEngine.API.Commands.Completion
{
    public sealed class CompletionEntry
    {
        public CompletionEntry(string id, string display = null, string description = null)
        {
            Id = id;

            Display = display ?? id;

            Description = description;
        }

        public readonly string Id;

        public readonly string Display;

        public readonly string Description;
    }

    public static class CompletionSources
    {
        private static int _generation;

        private static int _builtGeneration = -1;

        private static List<CompletionEntry> _items = [];

        private static List<CompletionEntry> _npcs = [];

        public static List<CompletionEntry> Items
        {
            get
            {
                Ensure();

                return _items;
            }
        }

        public static List<CompletionEntry> Npcs
        {
            get
            {
                Ensure();

                return _npcs;
            }
        }

        public static void Invalidate()
        {
            _generation++;

            _builtGeneration = -1;
        }

        private static void Ensure()
        {
            if (_builtGeneration == _generation && (_items.Count > 0 || _npcs.Count > 0))
                return;

            BuildItems();

            BuildNpcs();

            _builtGeneration = _generation;
        }

        private static void BuildItems()
        {
            _items = [];

            foreach (var item in ContentSamples.ItemsByType.Values)
            {
                if (item.IsAir)
                    continue;

                var id = item.ModItem is null
                    ? $"Terraria:{ItemID.Search.GetName(item.type)}"
                    : item.ModItem.FullName.Replace('/', ':');

                _items.Add(new CompletionEntry(id, item.Name));
            }
        }

        private static void BuildNpcs()
        {
            _npcs = [];

            foreach (var npc in ContentSamples.NpcsByNetId.Values)
            {
                var id = npc.ModNPC is null
                    ? $"Terraria:{NPCID.Search.GetName(npc.type)}"
                    : npc.ModNPC.FullName.Replace('/', ':');

                _npcs.Add(new CompletionEntry(id, npc.FullName));
            }
        }
    }
}
