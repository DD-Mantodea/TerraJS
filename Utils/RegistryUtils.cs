using Jint;
using Jint.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Items;

namespace TerraJS.Utils
{
    public class RegistryUtils
    {
        public static Type[] Parameters2Types(ParameterInfo[] paras)
        {
            Type[] types = new Type[paras.Length];
            var list = paras.ToList();
            foreach (var p in list)
            {
                types[list.IndexOf(p)] = p.ParameterType;
            }
            return types;
        }

        public static MethodBuilder CreateMethodBuilder(Type type, string methodName, TypeBuilder builder)
        {
            var method = type.GetMethod(methodName);

            return builder.DefineMethod(
                methodName,
                method.Attributes,
                method.CallingConvention,
                method.ReturnType,
                Parameters2Types(method.GetParameters())
            );
        }
    }
}
