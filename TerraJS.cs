using Terraria.ModLoader;
using Jint;
using System;
using TerraJS.API;
using System.IO;
using Jint.Runtime.Interop;
using System.Reflection;
using System.Dynamic;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using System.Linq;
using TerraJS.API.Events;
using TerraJS.API.Items;
using Terraria;

namespace TerraJS
{
	public class TerraJS : Mod
    {
        public static Engine Engine;

        public static TerraJS Instance;

        public static GlobalAPI GlobalAPI;

        static string ModPath => Path.Combine(Main.SavePath,"Mods", "TerraJS");

        public override void Load()
        {
            Instance = this;

            InitializeEngine();

            LoadAllScripts();

            GlobalAPI.Event.InvokeEvent("ModLoad");

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetOrRegister"), RedirectLocalizedText);
            
            MonoModHooks.Add(typeof(LocalizationLoader).GetMethod("UpdateLocalizationFilesForMod", BindingFlags.Static | BindingFlags.NonPublic), (Action<Mod, string, GameCulture> orig, Mod mod, string str, GameCulture cul) => 
            {
                if (mod is not TerraJS) 
                    orig.Invoke(mod, str, cul);
            });
        }

        public override void PostSetupContent()
        {
            GlobalAPI.Event.InvokeEvent("PostSetupContent");
        }

        public void InitializeEngine()
        {
            Engine = new Engine();

            GlobalAPI = new();

            BindInstance("TerraJS", GlobalAPI);

            BindStatic("ItemID", typeof(ItemID));

            BindStatic("AmmoID", typeof(AmmoID));

            BindStatic("ItemUseStyleID", typeof(ItemUseStyleID));

            BindEnum("Cultures", typeof(GameCulture.CultureName));

            BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));
        }

        public void BindProperties(string name, PropertyInfo[] members)
        {
            dynamic expandoObj = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expandoObj;

            foreach (var property in members)
                expandoDict[property.Name] = property.GetValue(null);

            Engine.SetValue(name, expandoObj);
        }

        public void BindInstance(string name, object instance) => Engine.SetValue(name, instance);

        public void BindEnum(string name, Type type) => Engine.SetValue(name, type);

        public void BindStatic(string name, Type type) => Engine.SetValue(name, TypeReference.CreateTypeReference(Engine, type));

        static void CreateFolderIfNotExist(string path) 
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

        public LocalizedText RedirectLocalizedText(Func<LanguageManager, string, Func<string>, LocalizedText> orig, LanguageManager instance, string key, Func<string> makeDefaultValue)
        {
            if (key.StartsWith("Mods.TerraJS"))
            {
                if(!TranslationAPI.LocalizedTexts.TryGetValue(key, out string value))
                    value = TranslationAPI.DefaultLocalizedTexts[key];

                return typeof(LocalizedText).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => !c.IsPublic).First().Invoke([key, value]) as LocalizedText;
            }
            else return orig.Invoke(instance, key, makeDefaultValue);
        }

        public override void Unload()
        {
            foreach(var i in GlobalAPI.Event.Events.Values)
                i.Delegates.Clear();

            base.Unload();
        }
    }
}
