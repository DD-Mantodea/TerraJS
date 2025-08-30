using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jint.Runtime.Interop;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API;
using TerraJS.JSEngine.API.Items.DamageClasses;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraJS.API.Items
{
    public class ItemAPI : BaseAPI
    {
        public ItemRegistry? CreateItemRegistry(string name, string @namespace = "") => new(name, @namespace);

        public DamageClassRegistry? CreateDamageClassRegistry(string name, string @namespace = "") => new(name, @namespace);

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

        public int GetModItem(TypeReference type) => GetModItem(type.ReferenceType);

        public int GetTJSItem(string fullName)
        {
            if (ItemRegistry._contentTypes.TryGetValue($"TJSContents.Items.{fullName}", out var itemType))
                return itemType;

            return -1;
        }

        public DamageClass GetModDamageClass(Type type)
        {
            if (!type.IsSubclassOf(typeof(DamageClass)))
                return null;

            return ModContentUtils.GetInstance<DamageClass>(type);
        }

        public DamageClass GetModDamageClass(TypeReference type) => GetModDamageClass(type.ReferenceType);

        public DamageClass GetTJSDamageClass(string fullName)
        {
            if (DamageClassRegistry._damageClasses.TryGetValue($"TJSContents.DamageClasses.{fullName}", out var dmgClzType))
                return ModContentUtils.GetInstance<DamageClass>(dmgClzType);

            return null;
        }

        public void NewItem(IEntitySource source, Vector2 pos, int type, Vector2 randomBox = default, int stack = 1, bool noBroadcast = false, int prefixGiven = 0, bool noGrabDelay = false, bool reverseLookup = false)
            => Item.NewItem(source, pos, randomBox, type, stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);

        internal override void Unload()
        {

        }
    }
}
