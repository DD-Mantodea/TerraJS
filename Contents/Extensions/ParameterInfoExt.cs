using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
