using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.EntityArguments
{
    public class EntityAssignment
    {
        public delegate bool Assignment(object target, MemberInfo variable, string value);

        public static Dictionary<string, Assignment> Assignments = [];

        public static bool TrySetValue<T>(object target, MemberInfo variable, T value)
        {
            switch (variable)
            {
                case FieldInfo f:
                    var fieldType = f.FieldType;

                    if (fieldType != typeof(T))
                        return false;

                    f.SetValue(target, value);

                    break;
                case PropertyInfo p:
                    var propertyType = p.PropertyType;

                    if (propertyType != typeof(T))
                        return false;

                    p.SetValue(target, value);

                    break;
            }

            return true;
        }

        static EntityAssignment()
        {
            Assignments.Add(typeof(int).FullName, (target, variable, value) =>
            {
                if (!int.TryParse(value, out var val))
                    return false;

                return TrySetValue(target, variable, val);
            });

            Assignments.Add(typeof(string).FullName, (target, variable, value) =>
            {
                return TrySetValue(target, variable, value);
            });
        }
    }
}
