using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API;
using TerraJS.API.Items;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Items.DamageClasses
{
    public class DamageClassRegistry : Registry<DamageClass>
    {
        public static DamageClassRegistry Empty => new() { IsEmpty = true };

        internal static Dictionary<string, Type> _damageClasses = [];

        public DamageClassRegistry() { }

        public DamageClassRegistry(TypeBuilder builder)
        {
            _builder = builder;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, $"Mods.{_builder.FullName}.DisplayName", _builder.Name);
        }

        public DamageClassRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (IsEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public DamageClassRegistry SetDefaultStats(Action<Player> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "SetDefaultStats", @delegate);

            return this;
        }

        public DamageClassRegistry GetEffectInheritance(Func<DamageClass, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "GetEffectInheritance", @delegate);

            return this;
        }

        public DamageClassRegistry GetPrefixInheritance(Func<DamageClass, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "GetPrefixInheritance", @delegate);

            return this;
        }

        public DamageClassRegistry GetModifierInheritance(Func<DamageClass, StatInheritanceData> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "GetModifierInheritance", @delegate);

            return this;
        }

        public DamageClassRegistry ShowStatTooltipLine(Func<Player, string, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "ShowStatTooltipLine", @delegate);

            return this;
        }

        public DamageClassRegistry DisplayName(Func<LocalizedText> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "get_DisplayName", @delegate);

            return this;
        }

        public DamageClassRegistry UseStandardCritCalcs(Func<bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "get_UseStandardCritCalcs", @delegate);

            return this;
        }

        public override void Register()
        {
            if (IsEmpty) return;

            var dmgClzType = _builder.CreateType();

            var JSDmgClz = Activator.CreateInstance(dmgClzType) as TJSDamageClass;

            TJSMod.AddContent(JSDmgClz);

            _damageClasses.Add(_builder.FullName, dmgClzType);

            _tjsInstances.Add(JSDmgClz);

            AfterRegister?.Invoke(dmgClzType);
        }
    }
}
