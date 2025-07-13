using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace TerraJS.API
{
    public class TranslationAPI : BaseAPI
    {
        public TranslationAPI()
        {
            foreach (int val in Enum.GetValues(typeof(GameCulture.CultureName)))
            {
                if (val == 9999) continue;

                Translations.Add(GameCulture.FromLegacyId(val), []);
            }

            //SetTranslation(GameCulture.DefaultCulture, "Mods.TerraJS.QuestButton.HoverText", "Open quest panel");

            //SetTranslation(GameCulture.FromLegacyId(7), "Mods.TerraJS.QuestButton.HoverText", "打开任务面板");
        }

        public Dictionary<GameCulture, Dictionary<string, string>> Translations = [];

        public Dictionary<string, string> LocalizedTexts => Translations[Language.ActiveCulture];

        public  Dictionary<string, string> DefaultLocalizedTexts => Translations[GameCulture.DefaultCulture];

        public void SetTranslation(GameCulture gameCultrue, string key, string value)
        {
            if (Translations[gameCultrue].ContainsKey(key))
                Translations[gameCultrue][key] = value;

            Translations[gameCultrue].Add(key, value);
        }

        public string GetTranslation(string key, GameCulture gameCulture = null)
        {
            if ((gameCulture != null && Translations[gameCulture].TryGetValue(key, out var ret)) || LocalizedTexts.TryGetValue(key, out ret))
                return ret;

            return key;
        }

        internal override void Unload()
        {
            Translations.Clear();

            foreach (int val in Enum.GetValues(typeof(GameCulture.CultureName)))
            {
                if (val == 9999) continue;

                Translations.Add(GameCulture.FromLegacyId(val), []);
            }
        }
    }
}
