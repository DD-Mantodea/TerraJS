using System;
using System.Linq;
using TerraJS.JSEngine.API.Items;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.NPCs
{
    public class NPCAPI : BaseAPI
    {
        public int GetModNPC(string modName, string npcName)
        {
            if (modName == "TerraJS") return GetTJSNPC(npcName);

            if (ModLoader.TryGetMod(modName, out var mod))
            {
                var type = mod.GetType().Assembly.GetTypes().First(t => t.Name == npcName);

                if (type == null) return -1;

                return GetModNPC(type);
            }

            return -1;
        }

        public int GetModNPC(string fullName)
        {
            var data = fullName.Split(":");

            if (data.Length < 2)
                return -1;

            return GetModNPC(data[0], data[1]);
        }

        public int GetModNPC(Type type)
        {
            if (!type.IsSubclassOf(typeof(ModNPC))) return -1;

            var npcTypeMethod = typeof(ModContent).GetMethod("NPCType");

            return (int)npcTypeMethod.MakeGenericMethod(type).Invoke(null, []);
        }

        public int GetTJSNPC(string fullName)
        {
            return -1;
        }

        internal override void Unload()
        {
            
        }
    }
}
