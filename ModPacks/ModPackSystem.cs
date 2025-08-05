using System.IO;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.ModPacks
{
    public class ModPackSystem : ModSystem
    {
        public override void Load()
        {
            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, "TerraJS.UI.CreateNewModPack", "Create New Mod Pack");

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromLegacyId(7), "TerraJS.UI.CreateNewModPack", "创建新的整合包");

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, "TerraJS.UI.OpenModPackDir", "Open ModPack Folder");

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromLegacyId(7), "TerraJS.UI.OpenModPackDir", "打开整合包文件夹");

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, "TerraJS.UI.EnterModPackName", "Enter Mod Pack Name");

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromLegacyId(7), "TerraJS.UI.EnterModPackName", "输入整合包名称");

            FileUtils.CreateDirectoryIfNotExist(Pathes.ModPacksPath);
        }
    }
}
