using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
