using System;
using System.Reflection;
using TerraJS.JSEngine.API;
using TerraJS.JSEngine.API.DotNets.Extensions;
using TerraJS.JSEngine.API.Items;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.Plugins
{
    public class PluginRegistryInfo<T, API> where T : ModType where API : BaseAPI
    {
        private static ExtensionRegistry _registryExtension;

        public PluginRegistryInfo()
        {
            _registryExtension = new ExtensionRegistry($"{typeof(T).Name}Ext");
        }

        public void Add<Registry>(Func<string, string, Registry> factory) where Registry : IRegistry<T>
        {
            _registryExtension.CreateExtensionMethod(
                $"Create{typeof(Registry).Name}", 
                typeof(API), typeof(Registry), 
                [typeof(string), typeof(string)], factory, 
                [
                    new("name"), 
                    new("namespace", ParameterAttributes.HasDefault | ParameterAttributes.Optional, "")
                ]
            );
        }

        public void RegisterExtension() => _registryExtension.Register();
    }

    public class PluginRegistryInfos
    {
        public static PluginRegistryInfo<ModItem, ItemAPI> Item = new();

        public static void RegisterAll()
        {
            Item.RegisterExtension();
        }
    }
}
