using System;
using System.Reflection;
using Jint.Runtime.Interop;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.Contents.Extensions
{
    public static class PlayerExt
    {
        private static DamageClass GetDamageClass(Type type)
        {
            return ModContentUtils.GetInstance<DamageClass>(type);
        }

        public static float GetCritChance(this Player player, TypeReference damageClass)
        {
            return GetCritChance(player, damageClass.ReferenceType);
        }

        public static float GetCritChance(this Player player, Type damageClass)
        {
            var dmgClz = GetDamageClass(damageClass);

            return GetCritChance(player, dmgClz);
        }

        public static float GetCritChance(this Player player, DamageClass damageClass)
        {
            return player.GetCritChance(damageClass);
        }

        public static void SetCritChance(this Player player, TypeReference damageClass, float value)
        {
            SetCritChance(player, damageClass.ReferenceType, value);
        }

        public static void SetCritChance(this Player player, Type damageClass, float value)
        {
            var dmgClz = GetDamageClass(damageClass);

            SetCritChance(player, dmgClz, value);
        }

        public static void SetCritChance(this Player player, DamageClass damageClass, float value)
        {
            player.GetCritChance(damageClass) = value;
        }

        public static float GetAttackSpeed(this Player player, TypeReference damageClass)
        {
            return GetAttackSpeed(player, damageClass.ReferenceType);
        }

        public static float GetAttackSpeed(this Player player, Type damageClass)
        {
            var dmgClz = GetDamageClass(damageClass);

            return GetAttackSpeed(player, dmgClz);
        }

        public static float GetAttackSpeed(this Player player, DamageClass damageClass)
        {
            return player.GetAttackSpeed(damageClass);
        }

        public static void SetAttackSpeed(this Player player, TypeReference damageClass, float value)
        {
            SetAttackSpeed(player, damageClass.ReferenceType, value);
        }

        public static void SetAttackSpeed(this Player player, Type damageClass, float value)
        {
            var dmgClz = GetDamageClass(damageClass);

            SetAttackSpeed(player, dmgClz, value);
        }

        public static void SetAttackSpeed(this Player player, DamageClass damageClass, float value)
        {
            player.GetAttackSpeed(damageClass) = value;
        }

        public static float GetArmorPenetration(this Player player, TypeReference damageClass, int a)
        {
            return GetArmorPenetration(player, damageClass.ReferenceType);
        }

        public static float GetArmorPenetration(this Player player, Type damageClass)
        {
            var dmgClz = GetDamageClass(damageClass);

            return GetArmorPenetration(player, dmgClz);
        }

        public static float GetArmorPenetration(this Player player, DamageClass damageClass)
        {
            return player.GetArmorPenetration(damageClass);
        }

        public static void SetArmorPenetration(this Player player, TypeReference damageClass, float value)
        {
            SetArmorPenetration(player, damageClass.ReferenceType, value);
        }

        public static void SetArmorPenetration(this Player player, Type damageClass, float value)
        {
            var dmgClz = GetDamageClass(damageClass);

            SetArmorPenetration(player, dmgClz, value);
        }

        public static void SetArmorPenetration(this Player player, DamageClass damageClass, float value)
        {
            player.GetArmorPenetration(damageClass) = value;
        }

        public static StatModifier GetKnockback(this Player player, TypeReference damageClass)
        {
            return GetKnockback(player, damageClass.ReferenceType);
        }

        public static StatModifier GetKnockback(this Player player, Type damageClass)
        {
            var dmgClz = GetDamageClass(damageClass);

            return GetKnockback(player, dmgClz);
        }

        public static StatModifier GetKnockback(this Player player, DamageClass damageClass)
        {
            return player.GetKnockback(damageClass);
        }

        public static void SetKnockback(this Player player, TypeReference damageClass, StatModifier value)
        {
            SetKnockback(player, damageClass.ReferenceType, value);
        }

        public static void SetKnockback(this Player player, Type damageClass, StatModifier value)
        {
            var dmgClz = GetDamageClass(damageClass);

            SetKnockback(player, dmgClz, value);
        }

        public static void SetKnockback(this Player player, DamageClass damageClass, StatModifier value)
        {
            player.GetKnockback(damageClass) = value;
        }

        public static StatModifier GetDamage(this Player player, TypeReference damageClass)
        {
            return GetDamage(player, damageClass.ReferenceType);
        }

        public static StatModifier GetDamage(this Player player, Type damageClass)
        {
            var dmgClz = GetDamageClass(damageClass);

            return GetDamage(player, dmgClz);
        }

        public static StatModifier GetDamage(this Player player, DamageClass damageClass)
        {
            return player.GetDamage(damageClass);
        }

        public static void SetDamage(this Player player, TypeReference damageClass, StatModifier value)
        {
            SetDamage(player, damageClass.ReferenceType, value);
        }

        public static void SetDamage(this Player player, Type damageClass, StatModifier value)
        {
            var dmgClz = GetDamageClass(damageClass);

            SetDamage(player, dmgClz, value);
        }

        public static void SetDamage(this Player player, DamageClass damageClass, StatModifier value)
        {
            player.GetDamage(damageClass) = value;
        }
    }
}
