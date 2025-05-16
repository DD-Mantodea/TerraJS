using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API
{
    public abstract class Registry<T>
    {
        internal TypeBuilder _builder;

        internal string _texturePath = "";

        internal static Type _contentType = typeof(T);

        internal Mod TJSMod = TerraJS.Instance;

        internal static Dictionary<string, int> ContentTypes = new();

        public Dictionary<string, Delegate> _delegates = new();

        public bool isEmpty = false;

        public abstract void Register();
    }
}
