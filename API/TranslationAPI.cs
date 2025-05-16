using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace TerraJS.API
{
    public class TranslationAPI
    {
        static TranslationAPI()
        {
            foreach (int val in Enum.GetValues(typeof(GameCulture.CultureName)))
            {
                if (val == 9999) return;

                Translations.Add(GameCulture.FromLegacyId(val), new());
            }
        }

        public static Dictionary<GameCulture, Dictionary<string, string>> Translations = new();

        public static Dictionary<string, string> LocalizedTexts => Translations[Language.ActiveCulture];

        public static Dictionary<string, string> DefaultLocalizedTexts => Translations[GameCulture.DefaultCulture];

        public static void AddTranslation(GameCulture gameCultrue, string key, string value)
        {
            Translations[gameCultrue].Add(key, value);
        }
    }
}
