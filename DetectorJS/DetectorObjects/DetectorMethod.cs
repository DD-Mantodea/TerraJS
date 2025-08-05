using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using Terraria;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorMethod(MethodInfo method) : DetectorObject
    {
        public MethodInfo Method = method;

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

            if (Method.GetCustomAttribute<ExtensionAttribute>() != null)
                return $"\"{Method.Name}\"({paramTexts}): {Type2ClassName(Method.ReturnType, asParameter: true)}";

            if (Method.IsGenericMethod)
            {
                var genericParams = Method.GetGenericArguments();

                var genericTexts = string.Join(", ", genericParams.Select(t => t.Name));

                return $"{(Method.IsStatic ? "static " : "")}\"{Method.Name}\"<{genericTexts}>({paramTexts}): {Type2ClassName(Method.ReturnType, asParameter: true)}";
            }

            return $"{(Method.IsStatic ? "static " : "")}\"{Method.Name}\"({paramTexts}): {Type2ClassName(Method.ReturnType, asParameter: true)}";
        }

        public string Default2String(object @default)
        {
            switch (@default)
            {
                case string:
                    return $"\"{@default}\"";

                case null:
                    return "null";

                case false:
                    return "false";

                case true:
                    return "true";

                default:
                    return @default.ToString();
            }
        }
    }
}
