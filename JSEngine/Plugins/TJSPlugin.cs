using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;
using TerraJS.DetectorJS;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.Plugins
{
    public abstract class TJSPlugin : ModType
    {
        protected sealed override void Register()
        {
            ModTypeLookup<TJSPlugin>.Register(this);

            AddCustomType(GetType());
        }

        public static void AddCustomType(Type type) => TJSEngine.CustomTypes.TryAdd(type);
    }
}
