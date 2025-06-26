using Terraria.ModLoader;
using Jint;
using System;
using TerraJS.API;
using System.IO;
using System.Reflection;
using Terraria.ID;
using Terraria.Localization;
using System.Linq;
using Terraria;
using TerraJS.Utils;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.API.Commands.CommandArguments.BasicArguments;
using TerraJS.Managers;
using System.Collections.Generic;
using TerraJS.Attributes;
using Terraria.ModLoader.Core;
using TerraJS.DetectorJS;

namespace TerraJS
{
	public class TerraJS : Mod
    {
        public static Engine Engine;

        public static TerraJS Instance;

        public static GlobalAPI GlobalAPI;

        public static TranslationAPI TranslationAPI;

        public FontManager FontManager = new();
        public static string ModPath => Path.Combine(Main.SavePath, "Mods", "TerraJS");

        public override void Load()
        {
            Instance = this;

            InitializeEngine();

            LoadAllScripts();

            GlobalAPI.Event.InvokeEvent("ModLoad");

            CustomLoad();

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

            FontManager.Load();
        }

        public void CustomLoad()
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

            GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "reload"))
                .Execute((_, _) => ReloadAllScripts())
                .Register();

            GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "detect"))
                .Execute((_, _) => Detector.Detect())
                .Register();
        }

        public override void PostSetupContent()
        {
            GlobalAPI.Event.InvokeEvent("PostSetupContent");
        }

        public void InitializeEngine()
        {
            Engine = new Engine(cfg => {
                cfg.AllowClr();
            });

            GlobalAPI = new();

            TranslationAPI = new();

            BindingUtils.BindInstance("TerraJS", GlobalAPI);

            BindingUtils.BindStaticOrEnumOrConst("Main", typeof(Main));

            BindingUtils.BindStaticOrEnumOrConst("Cultures", typeof(GameCulture.CultureName));

            BindItemThings();

            BindTileThings();

            BindClasses();
        }

        private void BindItemThings()
        {
            BindingUtils.BindStaticOrEnumOrConst("ItemID", typeof(ItemID));

            BindingUtils.BindStaticOrEnumOrConst("AmmoID", typeof(AmmoID));

            BindingUtils.BindStaticOrEnumOrConst("ItemUseStyleID", typeof(ItemUseStyleID));

            BindingUtils.BindStaticOrEnumOrConst("ItemRarityID", typeof(ItemRarityID));

            BindingUtils.BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));
        }

        private void BindTileThings()
        {
            BindingUtils.BindStaticOrEnumOrConst("TileID", typeof(TileID));
        }

        private void BindClasses()
        {
            foreach (var type in AssemblyManager.GetLoadableTypes(GetType().Assembly))
            {
                if (type.GetCustomAttribute<BindToEngineAttribute>() is { })
                {
                    BindingUtils.BindStaticOrEnumOrConst(type.Name, type);
                }
            }
        }

        public static void CreateFolderIfNotExist(string path) 
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public void LoadAllScripts()
        {
            var modPath = ModPath;

            CreateFolderIfNotExist(modPath);

            CreateFolderIfNotExist(Path.Combine(modPath, "Scripts"));

            CreateFolderIfNotExist(Path.Combine(modPath, "Textures"));

            var files = Directory.GetFiles(Path.Combine(modPath, "Scripts"));

            foreach (var file in files)
            {
                string script = File.ReadAllText(file);

                Engine.Execute(script);
            }
        }

        public void ReloadAllScripts()
        {
            GlobalAPI.Reload();

            TranslationAPI.Reload();

            LoadAllScripts();
        }

        public LocalizedText RedirectLocalizedText(Func<LanguageManager, string, Func<string>, LocalizedText> orig, LanguageManager instance, string key, Func<string> makeDefaultValue)
        {
            if (key.StartsWith("Mods.TerraJS"))
            {
                if(!TranslationAPI.LocalizedTexts.TryGetValue(key, out string value))
                    value = TranslationAPI.DefaultLocalizedTexts[key];

                return typeof(LocalizedText).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => !c.IsPublic).First().Invoke([key, value]) as LocalizedText;
            }
            else return orig(instance, key, makeDefaultValue);
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

        public override void Unload()
        {
            foreach(var i in GlobalAPI.Event.Events.Values)
                i.Delegates.Clear();

            base.Unload();
        }
    }
}
