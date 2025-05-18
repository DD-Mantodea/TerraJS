using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace TerraJS.API
{
    public class TranslationAPI
    {
        static TranslationAPI()
        {
            foreach (int val in Enum.GetValues(typeof(GameCulture.CultureName)))
            {
                if (val == 9999) continue;

                Translations.Add(GameCulture.FromLegacyId(val), []);
            }

            AddTranslation(GameCulture.DefaultCulture, "Mods.TerraJS.QuestButton.HoverText", "Open quest panel");

            AddTranslation(GameCulture.FromLegacyId(7), "Mods.TerraJS.QuestButton.HoverText", "打开任务面板");
        }

        public static Dictionary<GameCulture, Dictionary<string, string>> Translations = [];

        public static Dictionary<string, string> LocalizedTexts => Translations[Language.ActiveCulture];

        public static Dictionary<string, string> DefaultLocalizedTexts => Translations[GameCulture.DefaultCulture];

        public static void AddTranslation(GameCulture gameCultrue, string key, string value)
        {
            Translations[gameCultrue].Add(key, value);
        }

        public static string GetTranslation(string key, GameCulture gameCulture = null)
        {
            if ((gameCulture != null && Translations[gameCulture].TryGetValue(key, out var ret)) || LocalizedTexts.TryGetValue(key, out ret))
                return ret;

            return key;
        }
    }
}
