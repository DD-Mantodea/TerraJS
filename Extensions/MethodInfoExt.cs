using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Extensions
{
    public static class MethodInfoExt
    {
        public static bool IsIllegal(this MethodInfo method)
        {
            return method.IsSpecialName || method.GetParameters().Any(p => p.IsUnsafe()) || method.ReturnParameter.IsUnsafe();
        }
    }
}
