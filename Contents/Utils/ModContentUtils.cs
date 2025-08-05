using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Runtime.Interop;
using Terraria.ModLoader;

namespace TerraJS.Contents.Utils
{
    public class ModContentUtils
    {
        public static T GetInstance<T>(TypeReference type)
        {
            return GetInstance<T>(type.ReferenceType);
        }

        public static T GetInstance<T>(Type type)
        {
            return (T)typeof(ModContent).GetMethod("GetInstance").MakeGenericMethod(type).Invoke(null, []);
        }
    }
}
