using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using TerraJS.API.Events;
using TerraJS.API.Items;
using TerraJS.API.Recipes;
using Terraria.ModLoader;
using TerraJS.Utils;
using Terraria;
using TerraJS.API.Projectiles;
using TerraJS.API.Commands;

namespace TerraJS.API
{
    public class GlobalAPI
    {
        public CommandAPI Command = new();

        public EventAPI Event = new();

        public ItemAPI Item = new();

        public RecipeAPI Recipe = new();

        public ProjectileAPI Projectile = new();

        internal static AssemblyName _an = new AssemblyName("TJSContents");

        internal static AssemblyBuilder _ab = AssemblyBuilder.DefineDynamicAssembly(_an, AssemblyBuilderAccess.RunAndCollect);

        internal static ModuleBuilder _mb = _ab.DefineDynamicModule(_an.Name);
    }
}
