using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TerraJS.API;
using TerraJS.API.Items;
using TerraJS.JSEngine.API.DotNets.Extensions;
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
