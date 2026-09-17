using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace TerraJS.JSEngine.API.World.Subworlds
{
    public class SubworldRegistry : ModTypeRegistry<TJSSubworld, SubworldRegistry>
    {
        public override string Namespace => "Subworlds";

        internal static Dictionary<string, TJSSubworld> _subworlds = [];

        private int _width;

        private int _height;

        private List<GenPass> _tasks;

        public SubworldRegistry(string name, string @namespace = "") : base(name, @namespace)
        {

        }

        public SubworldRegistry Size(int width, int height)
        {
            if (IsEmpty) return this;

            _width = width;

            _height = height;

            return this;
        }

        public SubworldRegistry Size(Vector2 size)
        {
            if (IsEmpty) return this;

            _width = (int)size.X;

            _height = (int)size.Y;

            return this;
        }

        public SubworldRegistry VanillaWorld()
        {
            if (IsEmpty) return this;

            _tasks = [.. WorldGen.VanillaGenPasses.Values];

            return this;
        }

        public override void Register(Mod mod)
        {
            if (IsEmpty) return;

            var subworldType = _builder.CreateType();

            var JSSubworld = Activator.CreateInstance(subworldType) as TJSSubworld;

            JSSubworld._height = _height;

            JSSubworld._width = _width;

            JSSubworld._tasks = _tasks; 

            mod.AddContent(JSSubworld);

            _subworlds.Add(_builder.FullName, JSSubworld);

            _tjsInstances.Add(JSSubworld);
        }
    }
}
