using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    public class EquipLoaderHook : ModSystem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(EquipLoader).GetMethod("AddEquipTexture"), AddEquipTexture);
        }

        private int AddEquipTexture(Func<Mod, string, EquipType, ModItem, string, EquipTexture, int> orig, Mod mod, string texture, EquipType type, ModItem item, string name, EquipTexture equipTexture)
        {
            if (Mod is TerraJS)
                return orig(mod, $"TerraJS/Assets/Textures/Item/Default{Enum.GetName(typeof(EquipType), type)}", type, item, name, equipTexture);
            else return orig(mod, texture, type, item, name, equipTexture);
        }
    }
}
