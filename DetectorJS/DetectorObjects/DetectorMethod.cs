using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorMethod(MethodInfo method, Type thisType) : DetectorObject
    {
        public MethodInfo Method = method;

        public Type ThisType = thisType;

        public override string Serialize()
        {
            var parameters = Method.GetParameters();

            var eventInfo = Method.GetCustomAttribute<EventInfoAttribute>();

            if (Method.GetCustomAttribute<ExtensionAttribute>() != null)
                parameters = [..parameters.Skip(1)];

            var paramTexts = string.Join(", ", parameters.Select(p =>
            {
                var @default = p.IsOptional ? " = " + Default2String(p.DefaultValue) : "";

                var pName = p.Name.IsNullOrWhiteSpaceNotEmpty() ? p.ParameterType.Name.LowerFirst() : p.Name;

                return $"{SpecialNameCheck(pName)}: {Type2ClassName(p.ParameterType, eventInfo?.ParameterNames, true)}{@default}";
            }));

            var returnType = Method.ReturnType;

            if (returnType.IsGenericParameter && parameters.Length > 0 && returnType == Method.GetParameters()[0].ParameterType)
                returnType = ThisType;

            if (Method.GetCustomAttribute<ExtensionAttribute>() != null)
                return $"\"{Method.Name}\"({paramTexts}): {Type2ClassName(returnType, asParameter: true)}";

            if (Method.IsGenericMethod)
            {
                var genericParams = Method.GetGenericArguments();

                var genericTexts = string.Join(", ", genericParams.Select(t => t.Name));

                return $"{(Method.IsStatic ? "static " : "")}\"{Method.Name}\"<{genericTexts}>({paramTexts}): {Type2ClassName(returnType, asParameter: true)}";
            }

            return $"{(Method.IsStatic ? "static " : "")}\"{Method.Name}\"({paramTexts}): {Type2ClassName(returnType, asParameter: true)}";
        }

        public string Default2String(object @default)
        {
            return @default switch
            {
                string => $"\"{@default}\"",
                char => $"'{@default}'",
                null => "null",
                false => "false",
                true => "true",
                _ => @default.ToString(),
            };
        }
    }
}
