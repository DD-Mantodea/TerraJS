using System.Linq;
using System.Reflection;

namespace TerraJS.Contents.Extensions
{
    public static class MethodInfoExt
    {
        public static bool IsIllegal(this MethodInfo method)
        {
            return method.IsSpecialName || method.GetParameters().Any(p => p.IsUnsafe()) || method.ReturnParameter.IsUnsafe();
        }
    }
}
