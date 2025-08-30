using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.Attributes;
using TerraJS.JSEngine;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.Hooks
{
    [HideToJS]
    public class LanguageManagerHook : ModSystem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetText"), GetText);

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetTextValue", [typeof(string)]), GetTextValue);

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetTextValue", [typeof(string), typeof(object)]), GetTextValue1);

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetTextValue", [typeof(string), typeof(object), typeof(object)]), GetTextValue2);

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetTextValue", [typeof(string), typeof(object), typeof(object), typeof(object)]), GetTextValue3);

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetTextValue", [typeof(string), typeof(object[])]), GetTextValue4);

            MonoModHooks.Add(typeof(LanguageManager).GetMethod("GetOrRegister"), GetOrRegister);
        }

        private LocalizedText GetText(Func<LanguageManager, string, LocalizedText> orig, LanguageManager self, string key)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text;

            else return orig(self, key);
        }

        private string GetTextValue(Func<LanguageManager, string, string> orig, LanguageManager self, string key)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text.Value;

            return orig(self, key);
        }

        private string GetTextValue1(Func<LanguageManager, string, object, string> orig, LanguageManager self, string key, object arg0)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text.Format(arg0);

            return orig(self, key, arg0);
        }

        private string GetTextValue2(Func<LanguageManager, string, object, object, string> orig, LanguageManager self, string key, object arg0, object arg1)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text.Format(arg0);

            return orig(self, key, arg0, arg1);
        }

        private string GetTextValue3(Func<LanguageManager, string, object, object, object, string> orig, LanguageManager self, string key, object arg0, object arg1, object arg2)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text.Format(arg0);

            return orig(self, key, arg0, arg1, arg2);
        }

        private string GetTextValue4(Func<LanguageManager, string, object[], string> orig, LanguageManager self, string key, params object[] args)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text.Format(args);

            return orig(self, key, args);
        }

        private LocalizedText GetOrRegister(Func<LanguageManager, string, Func<string>, LocalizedText> orig, LanguageManager self, string key, Func<string> makeDefaultValue)
        {
            if (TryGetTJSTranslation(key, out var text))
                return text;

            else return orig(self, key, makeDefaultValue);
        }

        private bool TryGetTJSTranslation(string key, out LocalizedText text)
        {
            text = null;

            if (!TJSEngine.GlobalAPI.Translation.LocalizedTexts.TryGetValue(key, out string value))
                TJSEngine.GlobalAPI.Translation.DefaultLocalizedTexts.TryGetValue(key, out value);

            if (value != null)
            {
                text = typeof(LocalizedText).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(c => !c.IsPublic).First().Invoke([key, value]) as LocalizedText;
                
                return true;
            }

            return false;
        }
    }
}
