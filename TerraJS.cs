using Terraria.ModLoader;
using Jint;
using System;
using TerraJS.API;
using System.IO;
using System.Reflection;
using Terraria.Localization;
using System.Linq;
using Terraria;
using TerraJS.API.Commands.CommandArguments.BasicArguments;
using Terraria.ModLoader.Core;
using TerraJS.DetectorJS;
using Jint.Native;
using TerraJS.Assets.Managers;
using TerraJS.Contents.Utils;
using TerraJS.Contents.Attributes;
using Microsoft.Xna.Framework;

namespace TerraJS
{
	public class TerraJS : Mod
    {
        public static Engine Engine;

        public static TerraJS Instance;

        public static GlobalAPI GlobalAPI;

        public static TranslationAPI TranslationAPI;

        public static FontManager FontManager = new();

        public static TextureManager TextureManager = new();

        public static bool IsLoading => (bool)typeof(ModLoader).GetField("isLoading", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        public static string ModPath => Path.Combine(Main.SavePath, "Mods", "TerraJS");

        [HideToJS]
        public override void Load()
        {
            Instance = this;

            InitializeEngine();

            LoadAllScripts();

            GlobalAPI.Event.ModLoadEvent?.Invoke();

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
        }

        [HideToJS]
        public void Reload()
        {
            FontManager.Reload();

            TextureManager.Reload();

            ReloadAllScripts();

            GlobalAPI.Event.InGameReloadEvent?.Invoke();

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

            GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "reload"))
                .Execute((_, _) => Reload())
                .Register();

            GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "detect"))
                .Execute((_, _) => Detector.Detect())
                .Register();
        }

        [HideToJS]
        public override void PostSetupContent()
        {
            FontManager.Load();

            TextureManager.Load();

            GlobalAPI.Event.PostSetupContentEvent?.Invoke();
        }

        [HideToJS]
        public void InitializeEngine()
        {
            Assembly[] assemblies = [
                ..AssemblyManager.GetModAssemblies("TerraJS"), 
                typeof(ModLoader).Assembly,
                typeof(Vector2).Assembly
            ];

            Engine = new Engine(option => {
                option.AllowClr(assemblies);
                option.CatchClrExceptions(exception =>
                {
                    Console.WriteLine($"[Jint] CLR Exception: {exception.Message}");
                    return true;
                });
            });

            GlobalAPI = new();

            TranslationAPI = new();

            BindingUtils.BindInstance("TJS", GlobalAPI);

            BindingUtils.BindStaticOrEnumOrConst("Cultures", typeof(GameCulture.CultureName));

            BindingUtils.BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));

            BindInnerMethods();
        }

        [HideToJS]
        private void BindInnerMethods()
        {
            Engine.SetValue("require", require);
        }

        [HideToJS]
        public static void CreateFolderIfNotExist(string path) 
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        [HideToJS]
        public void LoadAllScripts()
        {
            CreateFolderIfNotExist(ModPath);

            CreateFolderIfNotExist(Path.Combine(ModPath, "Scripts"));

            CreateFolderIfNotExist(Path.Combine(ModPath, "Textures"));

            var files = Directory.GetFiles(Path.Combine(ModPath, "Scripts"), "*.js", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string script = File.ReadAllText(file);

                Engine.Execute(script);
            }
        }

        public void ReloadAllScripts()
        {
            Assembly[] assemblies = [
                ..AssemblyManager.GetModAssemblies("TerraJS"),
                typeof(ModLoader).Assembly,
                typeof(Vector2).Assembly
            ];

            Engine = new Engine(option => {
                option.AllowClr(assemblies);
                option.CatchClrExceptions(exception =>
                {
                    Console.WriteLine($"[Jint] CLR Exception: {exception.Message}");
                    return true;
                });
            });

            BindingUtils.BindInstance("TJS", GlobalAPI);

            BindingUtils.BindStaticOrEnumOrConst("Cultures", typeof(GameCulture.CultureName));

            BindingUtils.BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));

            BindInnerMethods();

            GlobalAPI.Unload();

            TranslationAPI.Unload();

            LoadAllScripts();
        }

        [HideToJS]
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

        private JsValue require(string path)
        {
            return Engine.Evaluate($"importNamespace(\"{path}\")");
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
            GlobalAPI.Unload();

            TranslationAPI.Unload();

            base.Unload();
        }
    }
}
