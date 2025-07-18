﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Extensions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraJS.API.Items
{
    public class ItemAPI : BaseAPI
    {
        public ItemRegistry? CreateItemRegistry(string name, string @namespace = "")
        {
            if (string.IsNullOrWhiteSpace(name) || @namespace.IsNullOrWhiteSpaceNotEmpty())
            {
                return ItemRegistry.Empty;
            }

            var itemName = $"TerraJS.Items.{(@namespace == "" ? "" : @namespace + ".")}{name}";

            TypeBuilder builder = GlobalAPI._mb.DefineType(itemName, TypeAttributes.Public, typeof(TJSItem));

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

            if (!ItemRegistry._contentTypes.TryGetValue($"TerraJS.Items.{fullName}", out itemType)) itemType = -1;

            return itemType;
        }

        public void NewItem(IEntitySource source, Vector2 pos, int type, Vector2 randomBox = default, int stack = 1, bool noBroadcast = false, int prefixGiven = 0, bool noGrabDelay = false, bool reverseLookup = false)
            => Item.NewItem(source, pos, randomBox, type, stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);

        internal override void Unload()
        {

        }
    }
}
