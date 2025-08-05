using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.Attributes;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    [HideToJS]
    public class LocalizationLoaderHook : ModSystem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(LocalizationLoader).GetMethod("UpdateLocalizationFilesForMod", BindingFlags.Static | BindingFlags.NonPublic), (Action<Mod, string, GameCulture> orig, Mod mod, string str, GameCulture cul) =>
            {
                if (mod is not TerraJS)
                    orig.Invoke(mod, str, cul);
            });
        }
    }
}
