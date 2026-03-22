using Jint.Native;
using System;
using System.Reflection;
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

        public static Type DefaultTypeConverter = typeof(JsValue).Assembly.GetType("Jint.Runtime.Interop.DefaultTypeConverter");

        public static Type InteropHelper = typeof(JsValue).Assembly.GetType("Jint.Runtime.Interop.InteropHelper");
    }
}
