using System;
using System.Reflection;

namespace TerraJS.Contents.Utils
{
    public class ReflectionUtils
    {
        public static object CallGeneric(MethodInfo method, Type type, object self, object[] parameters)
        {
            return method.MakeGenericMethod(type).Invoke(self, parameters);
        }
    }
}
