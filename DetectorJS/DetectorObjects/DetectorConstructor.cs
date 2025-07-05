using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorConstructor(ConstructorInfo constructor) : DetectorObject
    {
        public ConstructorInfo Constructor = constructor;

        public override string Serialize()
        {
            var parameters = Constructor.GetParameters().ToList();

            if (parameters.Exists(p => p.ParameterType.IsIllegal()))
                return "";

            var paramTexts = string.Join(", ", parameters.Select(p =>
            {
                var @default = p.IsOptional ? " = " + Default2String(p.DefaultValue) : "";

                return $"{SpecialNameCheck(p.Name)}: {Type2ClassName(p.ParameterType, asParameter: true)}{@default}";
            }));

            return $"constructor({paramTexts})";
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
