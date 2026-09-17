using System.Linq;
using System.Reflection;
using TerraJS.Contents.Extensions;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorConstructor(ConstructorInfo constructor) : DetectorObject
    {
        public ConstructorInfo Constructor = constructor;

        public override string Serialize()
        {
            var parameters = Constructor.GetParameters();

            if (parameters.Any(p => p.ParameterType.IsIllegal()))
                return "";

            var paramTexts = string.Join(", ", parameters.Select(p =>
            {
                var @default = p.IsOptional ? " = " + Default2String(p.DefaultValue) : "";

                return $"{SpecialNameCheck(p.Name)}: {Type2ClassName(p.ParameterType, asParameter: true)}{@default}";
            }));

            return $"constructor({paramTexts})";
        }
    }
}
