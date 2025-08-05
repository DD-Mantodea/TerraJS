using System.Reflection;
using TerraJS.JSEngine;
using TerraJS.ModPacks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraJS.Hooks
{
    public class InterfaceHook : ModSystem
    {
        public override void Load()
        {
            var method = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetMethod("ModLoaderMenus", BindingFlags.NonPublic | BindingFlags.Static);

            MonoModHooks.Add(method, ModLoaderMenus);

            On_UIWorkshopHub.OnInitialize += On_UIWorkshopHub_OnInitialize;

            On_UIWorkshopHub.Click_OpenModPackMenu += On_UIWorkshopHub_Click_OpenModPackMenu;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, "TerraJS.UI.ModPackDescription", "Manage TerraJS Mod Packs, right click to open vanilla UI.");

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromLegacyId(7), "TerraJS.UI.ModPackDescription", "管理TerraJS整合包，右键打开原版界面。");
        }

        private delegate void InterfaceModLoaderMenusDelegate(Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, int[] buttonVerticalSpacing, ref int offY, ref int spacing, ref int numButtons, ref bool backButtonDown);
        private void ModLoaderMenus(InterfaceModLoaderMenusDelegate orig, Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, int[] buttonVerticalSpacing, ref int offY, ref int spacing, ref int numButtons, ref bool backButtonDown)
        {
            orig(main, selectedMenu, buttonNames, buttonScales, buttonVerticalSpacing, ref offY, ref spacing, ref numButtons, ref backButtonDown);
            
            if (Main.menuMode == 42323)
            {
                Main.MenuUI.SetState(UITJSModPackMenu.Instance);
                Main.menuMode = MenuID.FancyUI; // 888
            }
        }

        private void On_UIWorkshopHub_OnInitialize(On_UIWorkshopHub.orig_OnInitialize orig, UIWorkshopHub self)
        {
            orig(self);

            var buttonModPack = typeof(UIWorkshopHub).GetField("_buttonModPack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self) as UIElement;

            buttonModPack.OnMouseOver += (_, _) =>
            {
                var text = typeof(UIWorkshopHub).GetField("_descriptionText", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) as UIText;

                text.SetText(Language.GetTextValue("TerraJS.UI.ModPackDescription"));
            };

            buttonModPack.OnRightClick += (e, el) =>
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);

                var modPacksMenu = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetField("modPacksMenu", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as UIState;

                (modPacksMenu as IHaveBackButtonCommand).PreviousUIState = self;

                Main.MenuUI.SetState(modPacksMenu);
            };
        }

        private void On_UIWorkshopHub_Click_OpenModPackMenu(On_UIWorkshopHub.orig_Click_OpenModPackMenu orig, UIWorkshopHub self, UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);

            UITJSModPackMenu.Instance.PreviousUIState = self;

            Main.MenuUI.SetState(UITJSModPackMenu.Instance);
        }
    }
}
