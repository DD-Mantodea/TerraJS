﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using TerraJS.API.Events;
using TerraJS.API.Items;
using TerraJS.API.Recipes;
using TerraJS.API.Projectiles;
using TerraJS.API.Commands;
using TerraJS.API.Tiles;
using TerraJS.API.Players;
using TerraJS.API.Reflections;
using TerraJS.API.NPCs;

namespace TerraJS.API
{
    public class GlobalAPI : BaseAPI
    {
        public CommandAPI Command = new();

        public EventAPI Event = new();

        public ItemAPI Item = new();

        public RecipeAPI Recipe = new();

        public ProjectileAPI Projectile = new();

        public TileAPI Tile = new();

        public PlayerAPI Player = new();

        public ReflectionAPI Reflection = new();

        public TranslationAPI Translation = new();

        public NPCAPI NPC = new();

        internal static AssemblyName _an = new AssemblyName("TJSContents");

        internal static AssemblyBuilder _ab = AssemblyBuilder.DefineDynamicAssembly(_an, AssemblyBuilderAccess.RunAndCollect);

        internal static ModuleBuilder _mb = _ab.DefineDynamicModule(_an.Name);

        public void Info(object obj) => TerraJS.Instance.Logger.Info(obj);

        public void Debug(object obj) => TerraJS.Instance.Logger.Debug(obj);

        public void Warn(object obj) => TerraJS.Instance.Logger.Warn(obj);

        internal override void Unload()
        {
            Command.Unload();
            Event.Unload();
            Item.Unload();
            Recipe.Unload();
            Projectile.Unload();
            Tile.Unload();
            Player.Unload();
            Reflection.Unload();
            Translation.Unload();
        }
    }
}
