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

            BindItemThings();

            BindEnumOrConst("Cultures", typeof(GameCulture.CultureName));
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

        public void BindEnumOrConst(string name, Type type) => Engine.SetValue(name, type);

        public void BindStatic(string name, Type type) => Engine.SetValue(name, TypeReference.CreateTypeReference(Engine, type));

        public void BindItemThings()
        {
            BindStatic("ItemID", typeof(ItemID));

            BindStatic("AmmoID", typeof(AmmoID));

            BindStatic("ItemUseStyleID", typeof(ItemUseStyleID));

            BindEnumOrConst("ItemRarityID", typeof(ItemRarityID));

            BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));
        }

        public void LoadAllScripts()
        {
            if (!Directory.Exists("./TerraJS")) Directory.CreateDirectory("./TerraJS");

            if (!Directory.Exists("./TerraJS/Scripts")) Directory.CreateDirectory("./TerraJS/Scripts");

            if (!Directory.Exists("./TerraJS/Textures")) Directory.CreateDirectory("./TerraJS/Textures");

            foreach (var file in Directory.GetFiles("./TerraJS/Scripts"))
            {
                string script = File.ReadAllText(file);

                Engine.Execute(script);
            }
        }

        public LocalizedText RedirectLocalizedText(Func<LanguageManager, string, Func<string>, LocalizedText> orig, LanguageManager instance, string key, Func<string> makeDefaultValue)
        {
            if (key.StartsWith("Mods.TerraJS"))
            {
                var value = TranslationAPI.LocalizedTexts.ContainsKey(key) ? TranslationAPI.LocalizedTexts[key] : TranslationAPI.DefaultLocalizedTexts[key];

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
