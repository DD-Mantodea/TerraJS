using System;
using System.Collections.Generic;
using System.IO;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Items.DamageClasses
{
    public class DamageClassRegistry : ModTypeRegistry<TJSDamageClass, DamageClassRegistry>
    {
        internal static Dictionary<string, Type> _damageClasses = [];

        public override string Namespace => "DamageClasses";

        public DamageClassRegistry(string name, string @namespace = "") : base(name, @namespace)
        {
            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.DefaultCulture, $"Mods.{_builder.FullName}.DisplayName", _builder.Name);
        }

        public DamageClassRegistry Name(GameCulture.CultureName gameCulture, string str)
        {
            if (IsEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Mods.{_builder.FullName}.DisplayName", str);

            return this;
        }

        public DamageClassRegistry SetDefaultStats(Action<TJSDamageClass, Player> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "SetDefaultStats", @delegate);

            return this;
        }

        public DamageClassRegistry GetEffectInheritance(Func<TJSDamageClass, DamageClass, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "GetEffectInheritance", @delegate);

            return this;
        }

        public DamageClassRegistry GetPrefixInheritance(Func<TJSDamageClass, DamageClass, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "GetPrefixInheritance", @delegate);

            return this;
        }

        public DamageClassRegistry GetModifierInheritance(Func<TJSDamageClass, DamageClass, StatInheritanceData> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "GetModifierInheritance", @delegate);

            return this;
        }

        public DamageClassRegistry ShowStatTooltipLine(Func<TJSDamageClass, Player, string, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "ShowStatTooltipLine", @delegate);

            return this;
        }

        public DamageClassRegistry DisplayName(Func<TJSDamageClass, LocalizedText> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "get_DisplayName", @delegate);

            return this;
        }

        public DamageClassRegistry UseStandardCritCalcs(Func<TJSDamageClass, bool> @delegate)
        {
            if (IsEmpty)
                return this;

            RegistryUtils.Override(this, "get_UseStandardCritCalcs", @delegate);

            return this;
        }

        public override void Register(Mod mod)
        {
            if (IsEmpty) return;

            var dmgClzType = _builder.CreateType();

            var JSDmgClz = Activator.CreateInstance(dmgClzType) as TJSDamageClass;

            mod.AddContent(JSDmgClz);

            _damageClasses.Add(_builder.FullName, dmgClzType);

            _tjsInstances.Add(JSDmgClz);

            AfterRegister?.Invoke(dmgClzType);
        }
    }
}
