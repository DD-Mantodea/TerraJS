using System.Reflection;

namespace TerraJS.Contents.Extensions
{
    public static class ParameterInfoExt
    {
        public static bool IsUnsafe(this ParameterInfo parameter)
        {
            return parameter.ParameterType.IsPointer || parameter.ParameterType.IsByRef;
        }
    }
}
