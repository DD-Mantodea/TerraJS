using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace TerraJS.Extensions
{
    public static class TypeExt
    {
        public static bool IsGlobalNamespace(this Type type)
        {
            if (type == null)
                return false;

            if (type.IsIllegal())
                return false;

            return string.IsNullOrEmpty(type.Namespace);
        }

        public static bool IsDynamicType(this Type type)
        {
            return typeof(System.Reflection.Emit.TypeBuilder).IsAssignableFrom(type.GetType()) ||
                   type.Assembly.IsDynamic;
        }

        public static bool IsCompilerGenerated(this Type type)
        {
            return type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
        }

        public static bool IsPrivateImplementationDetails(this Type type)
        {
            return (type.FullName?.Contains("<PrivateImplementationDetails>")) ?? false;
        }

        public static bool IsIllegal(this Type type)
        {
            if (type.IsPointer ||
                type.IsByRef ||
                type.IsGenericParameter ||
                type.IsConstructedGenericType ||
                type.ContainsGenericParameters ||
                type.IsSpecialName ||
                type.IsCompilerGenerated() ||
                type.IsDynamicType() ||
                type.IsPrivateImplementationDetails()
                )
            {
                return true;
            }

            return false;
        }
    }
}
