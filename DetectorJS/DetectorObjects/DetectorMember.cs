using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorMember(MemberInfo info) : DetectorObject
    {
        public MemberInfo MemberInfo = info;

        public override string Serialize()
        {
            switch (MemberInfo)
            {
                case PropertyInfo property:
                    return $"{(property.SetMethod == null ? "readonly " : "")}" +
                        $"{(property.GetMethod.IsStatic ? "static " : "")}" +
                        $"\"{MemberInfo.Name}\": {Type2ClassName(property.PropertyType)}";

                case FieldInfo field:
                    return $"{(field.IsInitOnly ? "readonly " : "")}" +
                        $"{(field.IsStatic ? "static " : "")}" +
                        $"\"{MemberInfo.Name}\": {Type2ClassName(field.FieldType)}";

                case MethodInfo method:
                    return new DetectorMethod(method).Serialize();

                default:
                    return "";
            }
        }
    }
}
