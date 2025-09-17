using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.DetectorJS;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.Plugins
{
    [Autoload(false)]
    public abstract class TJSPlugin : ModType
    {
        protected sealed override void Register() { }

        public virtual void AddBindings() { }

        public virtual void AddEventAPIs() { }

        public static void BindInstance(string name, object instance) => BindingUtils.BindInstance(name, instance);

        public static void BindMethod(string name, Delegate @delegate) => BindingUtils.BindInnerMethod(name, @delegate);

        public static void BindEventAPI<T>(string name, T instance) where T : BaseEventAPI => BindInstance(name, instance);
    }
}
