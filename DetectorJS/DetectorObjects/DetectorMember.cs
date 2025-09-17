using System.Reflection;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorMember(MemberInfo info) : DetectorObject
    {
        public MemberInfo MemberInfo = info;

        public override string Serialize()
        {
            if (MemberInfo.GetCustomAttribute<HideToJSAttribute>() != null)
                return "";

            switch (MemberInfo)
            {
                case PropertyInfo property:
                    if (property.GetMethod == null || property.PropertyType.IsIllegal())
                        return "";
                    
                    return $"{(property.SetMethod == null ? "readonly " : "")}" +
                        $"{(property.GetMethod.IsStatic ? "static " : "")}" +
                        $"\"{MemberInfo.Name}\": {Type2ClassName(property.PropertyType)}";

                case FieldInfo field:
                    if (field.FieldType.IsIllegal())
                        return "";

                    return $"{(field.IsInitOnly ? "readonly " : "")}" +
                        $"{(field.IsStatic ? "static " : "")}" +
                        $"\"{MemberInfo.Name}\": {Type2ClassName(field.FieldType)}";

                case MethodInfo method:
                    foreach (var p in method.GetParameters())
                        if (p.ParameterType.IsIllegal())
                            return "";

                    return new DetectorMethod(method).Serialize();

                case ConstructorInfo constructor:
                    return new DetectorConstructor(constructor).Serialize();

                case EventInfo @event:
                    return new DetectorEvent(@event).Serialize();

                default:
                    return "";

            }
        }
    }
}
