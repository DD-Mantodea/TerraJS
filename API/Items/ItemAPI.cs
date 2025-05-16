using Jint.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Items
{
    public class ItemAPI
    {
        public static Dictionary<string, Dictionary<string, Delegate>> ItemDelegates = [];

        public ItemRegistry? CreateItemRegistry(string name, string @namespace = "")
        {
            var itemName = $"TerraJS.Items.{(@namespace == "" ? "" : @namespace + ".")}{name}";

            TypeBuilder builder = GlobalAPI._mb.DefineType(itemName, TypeAttributes.Public, typeof(JSItem));

            var registry = new ItemRegistry(builder);

            return registry;
        }

        public int GetModItem(string modName, string itemName)
        {
            if (modName == "TerraJS") return GetTJSItem(itemName);

            if (ModLoader.TryGetMod(modName, out var mod))
            {
                var type = mod.GetType().Assembly.GetTypes().First(t => t.Name == itemName);

                if (type == null) return -1;

                return GetModItem(type);
            }

            return -1;
        }

        public int GetModItem(Type type)
        {
            if (!type.IsSubclassOf(typeof(ModItem))) return -1;

            var itemTypeMethod = typeof(ModContent).GetMethod("ItemType");

            return (int)itemTypeMethod.MakeGenericMethod(type).Invoke(null, []);
        }


        public int GetTJSItem(string fullName)
        {
            int itemType = -1;

            if (!ItemRegistry.ContentTypes.TryGetValue($"TerraJS.Items.{fullName}", out itemType)) itemType = -1;

            return itemType;
        }
    }
}
