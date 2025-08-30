using System.Reflection;
using System.Reflection.Emit;
using TerraJS.API;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.DotNets.Enums;

namespace TerraJS.JSEngine.API.DotNets
{
    public class DotNetAPI : BaseAPI
    {
        /*
        public EnumRegistry CreateEnumRegistry(string name, string @namespace = "")
        {
            if (string.IsNullOrWhiteSpace(name) || @namespace.IsNullOrWhiteSpaceNotEmpty())
            {
                return Enums.EnumRegistry.Empty;
            }

            var enumName = $"TJSContents.Enums.{(@namespace == "" ? "" : @namespace + ".")}{name}";

            EnumBuilder builder = GlobalAPI._mb.DefineEnum(enumName, TypeAttributes.Public, typeof(int));

            var registry = new EnumRegistry(builder);

            return registry;
        }
        */

        internal override void Unload()
        {

        }
    }
}
