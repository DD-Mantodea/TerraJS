using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace TerraJS.JSEngine.API.World.Subworlds
{
    public abstract class TJSSubworld : Subworld
    {
        public override int Width => _width;

        internal int _width;

        public override int Height => _height;

        internal int _height;

        public override List<GenPass> Tasks => _tasks;

        internal List<GenPass> _tasks = [];
    }
}
