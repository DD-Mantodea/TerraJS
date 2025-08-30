using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TerraJS.Contents.Utils
{
    public class TypeUtils
    {
        public static Type ModOrganizer = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");

        public static Type Interface = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface");

        public static Type UILoaderAnimatedImage = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UILoaderAnimatedImage");

        public static Type BuildProperties = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Core.BuildProperties");

        public static Type ModReference = BuildProperties.GetNestedType("ModReference", BindingFlags.NonPublic);
    }
}
