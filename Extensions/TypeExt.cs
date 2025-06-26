using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Extensions
{
    public static class TypeExt
    {
        public static bool IsGlobalNamespace(this Type type)
        {
            if (type == null)
                return false;

            if (type.IsArray ||
                type.IsPointer ||
                type.IsGenericParameter ||
                type.IsConstructedGenericType ||
                type.ContainsGenericParameters ||
                type.IsAnonymousType() ||
                type.IsDynamicType() ||
                type.IsSpecialName)
            {
                return false;
            }

            return string.IsNullOrEmpty(type.Namespace);
        }
        public static bool IsAnonymousType(this Type type)
        {
            return type.Name.Contains("<>") &&
                   type.Name.Contains("AnonymousType");
        }
        public static bool IsDynamicType(this Type type)
        {
            return typeof(System.Reflection.Emit.TypeBuilder).IsAssignableFrom(type.GetType()) ||
                   type.Assembly.IsDynamic;
        }
        public static bool IsFixedBufferType(this Type type)
        {
            return type.Name.StartsWith("<") &&
                   type.Name.Contains(">e__FixedBuffer");
        }
    }
}
