using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TerraJS.DetectorJS.DetectorObjects;

namespace TerraJS.Contents.Extensions
{
    public static class TypeExt
    {
        public static Regex SourceGeneratedRegex = new(@"<[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}>[A-Za-z_][A-Za-z0-9_]*");

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
            return (typeof(System.Reflection.Emit.TypeBuilder).IsAssignableFrom(type.GetType()) ||
                   type.Assembly.IsDynamic) && !type.Assembly.FullName.Contains("TJSContents");
        }

        public static bool IsCompilerGenerated(this Type type)
        {
            return type.GetCustomAttribute<CompilerGeneratedAttribute>() != null || 
                (type.FullName?.Contains("<>c__DisplayClass") ?? false) ||
                (type.FullName?.Contains("<>z__ReadOnly") ?? false);
        }

        public static bool IsSourceGenerated(this Type type)
        {
            return SourceGeneratedRegex.IsMatch(type.FullName ?? "");
        }

        public static bool IsPrivateImplementationDetails(this Type type)
        {
            return type.FullName?.Contains("<PrivateImplementationDetails>") ?? false;
        }

        public static bool IsInlineArray(this Type type)
        {
            return type.FullName?.Contains("<>y__InlineArray") ?? false;
        }

        public static bool IsExtension(this Type type)
        {
            return type.FullName?.Contains("<>E__") ?? false;
        }

        public static bool IsRegexGenerator(this Type type)
        {
            return type.FullName?.Contains("<RegexGenerator_g>") ?? false;
        }

        public static bool IsSearchValue(this Type type)
        {
            return type.FullName?.Contains("__CharSearchValues") ?? false;
        }

        public static bool IsIllegal(this Type type)
        {
            if (type.IsPointer ||
                type.IsByRef ||
                type.IsSpecialName ||
                type.IsCompilerGenerated() ||
                type.IsDynamicType() ||
                type.IsPrivateImplementationDetails() ||
                type.IsInlineArray() ||
                type.IsExtension() ||
                type.IsRegexGenerator() ||
                type.IsSourceGenerated() ||
                type.IsSearchValue() ||
                DetectorObject.Type2ClassName(type) == ""
                )
            {
                return true;
            }

            if (type.IsArray)
                return type.GetElementType().IsIllegal();

            return false;
        }
    }
}
