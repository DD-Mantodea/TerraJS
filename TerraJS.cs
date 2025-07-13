using Terraria.ModLoader;
using System;
using TerraJS.API;
using System.IO;
using System.Reflection;
using Terraria.Localization;
using System.Linq;
using Terraria;
using TerraJS.API.Commands.CommandArguments.BasicArguments;
using TerraJS.DetectorJS;
using TerraJS.Assets.Managers;
using TerraJS.Contents.Attributes;
using TerraJS.JSEngine;
using Terraria.ID;
using NetSimplified;
using TerraJS.Assets.AssetManagers;

namespace TerraJS
{
	public class TerraJS : Mod
    {
        public static TerraJS Instance;

        public static FontManager FontManager = new();

        public static TextureManager TextureManager = new();

        public static SHAManager SHAManager = new();

        public static bool IsLoading => (bool)typeof(ModLoader).GetField("isLoading", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        public static string ModPath => Path.Combine(Main.SavePath, "Mods", "TerraJS");

        [HideToJS]
        public override void Load()
        {
            Instance = this;

            TJSEngine.Load();

            TJSEngine.GlobalAPI.Event.ModLoadEvent?.Invoke();

            RegisterCommands();

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetOrRegister"), RedirectLocalizedText);
            /*
            MonoModHooks.Add(typeof(UserInterface).GetMethod("Update"), ModifyUpdate);

            MonoModHooks.Add(typeof(UserInterface).GetMethod("Draw"), ModifyDraw);

            MonoModHooks.Add(typeof(GameInterfaceLayer).GetMethod("Draw"), ModifyDrawLayer);
            */
            MonoModHooks.Add(typeof(LocalizationLoader).GetMethod("UpdateLocalizationFilesForMod", BindingFlags.Static | BindingFlags.NonPublic), (Action<Mod, string, GameCulture> orig, Mod mod, string str, GameCulture cul) => 
            {
                if (mod is not TerraJS) 
                    orig.Invoke(mod, str, cul);
            });

            AddContent<NetModuleLoader>();
        }

        [HideToJS]
        public void Reload()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                Main.NewText("请在单人模式下重载 TerraJS", 255, 0, 0);
                return;
            }

            FontManager.Load();

            TextureManager.Load();

            SHAManager.Load();

            TJSEngine.Load();

            TJSEngine.GlobalAPI.Event.InGameReloadEvent?.Invoke();

            Main.NewText("重载完成");
        }

        [HideToJS]
        public void RegisterCommands()
        {
            /*
            GlobalAPI.Command.CreateCommandRegistry("tjsquest")
                .NextArgument(new ComboArgument("value", ["edit"]))
                .Execute((g, _) =>
                {
                    var value = g.GetString("value");

                    switch (value)
                    {
                        case "edit":
                            QuestPanel.Instance.EditingMode = !QuestPanel.Instance.EditingMode;
                            break;
                    }
                })
                .Register();
            */

            TJSEngine.GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "reload"))
                .Execute((_, _) => Reload())
                .Register();

            TJSEngine.GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "detect"))
                .Execute((_, _) => Detector.Detect())
                .Register();
        }

        [HideToJS]
        public override void PostSetupContent()
        {
            FontManager.Load();

            TextureManager.Load();

            SHAManager.Load();

            TJSEngine.GlobalAPI.Event.PostSetupContentEvent?.Invoke();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetModule.ReceiveModule(reader, whoAmI);
        }

        [HideToJS]
        public LocalizedText RedirectLocalizedText(Func<LanguageManager, string, Func<string>, LocalizedText> orig, LanguageManager instance, string key, Func<string> makeDefaultValue)
        {
            if (!TJSEngine.GlobalAPI.Translation.LocalizedTexts.TryGetValue(key, out string value))
                TJSEngine.GlobalAPI.Translation.DefaultLocalizedTexts.TryGetValue(key, out value);

            if (value != null)
                return typeof(LocalizedText).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => !c.IsPublic).First().Invoke([key, value]) as LocalizedText;

            else return orig(instance, key, makeDefaultValue);
        }


        public override object Call(params object[] args)
        {
            if (args.Length == 0) return null;

            return null;
        }

        /*
        public void ModifyUpdate(Action<UserInterface, GameTime> orig, UserInterface ui, GameTime time)
        {
            if (QuestPanel.Instance != null && QuestPanel.Instance.Enabled) 
                return;

            orig(ui, time);
        }

        public void ModifyDraw(Action<UserInterface, SpriteBatch, GameTime> orig, UserInterface ui, SpriteBatch spriteBatch, GameTime time)
        {
            if (QuestPanel.Instance != null && QuestPanel.Instance.Enabled)
                return;

            orig(ui, spriteBatch, time);
        }

        public bool ModifyDrawLayer(Func<GameInterfaceLayer, bool> orig, GameInterfaceLayer layer)
        {
            if (layer.Name.StartsWith("Vanilla") && layer.Name != "Vanilla: Cursor" && QuestPanel.Instance != null && QuestPanel.Instance.Enabled)
                return true;

            return orig(layer);
        }
        */

        [HideToJS]
        public override void Unload()
        {
            TJSEngine.Unload();

            base.Unload();
        }
    }
}
