using System.Collections.Generic;
using System.Reflection;
using TerraJS.JSEngine;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace TerraJS.Hooks
{
    public unsafe class SystemLoaderHook : ModSystem
    {
        public override void Load()
        {
            var method = typeof(SystemLoader).GetMethod("ModifyWorldGenTasks", BindingFlags.Static | BindingFlags.Public);

            MonoModHooks.Add(method, ModifyWorldGenTasksHook);
        }

        public delegate void DelegateModifyWorldGenTasks(List<GenPass> passes, ref double totalWeight);

        private static void ModifyWorldGenTasksHook(DelegateModifyWorldGenTasks orig, List<GenPass> passes, ref double totalWeight)
        {
            fixed (double* pTotalWeight = &totalWeight)
            {
                TJSEngine.GlobalAPI.Event.World.ModifyWorldGenTasksEvent?.Invoke(passes, new(pTotalWeight));
            }

            orig(passes, ref totalWeight);
        }
    }
}
