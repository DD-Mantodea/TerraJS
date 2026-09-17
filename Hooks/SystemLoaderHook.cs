using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.WorldBuilding;

namespace TerraJS.Hooks
{
    public unsafe class SystemLoaderHook : ModSystem
    {
        public static HookList<ModSystem> HookModifyWorldGenTasks;

        public override void Load()
        {
            var method = typeof(SystemLoader).GetMethod("ModifyWorldGenTasks", BindingFlags.Static | BindingFlags.Public);

            MonoModHooks.Add(method, ModifyWorldGenTasksHook);
        }

        public delegate void DelegateModifyWorldGenTasks(List<GenPass> passes, ref double totalWeight);

        private static void ModifyWorldGenTasksHook(DelegateModifyWorldGenTasks orig, List<GenPass> passes, ref double totalWeight)
        {
            var hookModifyWorldGenTasks = typeof(SystemLoader).GetField("HookModifyWorldGenTasks", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(null) as HookList<ModSystem>;

            HookModifyWorldGenTasks = hookModifyWorldGenTasks;

            foreach (var system in hookModifyWorldGenTasks.Enumerate())
            {
                try
                {
                    var mod = system.Mod;

                    if (!TJSEngine.GlobalAPI.Event.World.ShouldModWorldGenEvent?.Invoke(mod) ?? true)
                        continue;

                    system.ModifyWorldGenTasks(passes, ref totalWeight);
                }
                catch (Exception e)
                {
                    string message = string.Join(
                        "\n",
                        system.FullName + " : " + Language.GetTextValue("tModLoader.WorldGenError"),
                        e
                    );
                    Utils.ShowFancyErrorMessage(message, 0);

                    throw;
                }
            }
            
            TJSEngine.GlobalAPI.Event.World.ModifyWorldGenTasksEvent?.Invoke(passes, new(totalWeight));
        }
    }
}
