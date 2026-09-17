using System;
using System.Collections.Generic;
using System.Reflection;
using TerraJS.JSEngine.API.Events.Ref;
using TerraJS.JSEngine.API.World.Subworlds;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace TerraJS.JSEngine.API.World
{
    public class WorldAPI : BaseAPI
    {
        public SubworldRegistry CreateSubworldRegistry(string name, string @namespace = "") => new(name, @namespace);

        public void AddGenPassToSubworld(string fullName, GenPass genPass)
        {
            var subworld = GetTJSSubworld(fullName);

            if (subworld is null)
                return;

            subworld.Tasks.Add(genPass);
        }

        public TJSSubworld GetTJSSubworld(string fullName)
        {
            if (SubworldRegistry._subworlds.TryGetValue($"TJSContents.Subworlds.{fullName}", out var subworld))
                return subworld;

            return null;
        }

        public void ModPassesToSubworld(Mod mod)
        {
            
        }

        internal override void Unload()
        {

        }
    }
}
