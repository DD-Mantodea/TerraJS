using System;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.CommandArguments.SelectorArguments;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.Plugins
{
    [Autoload(false)]
    public abstract class TJSPlugin : ModType
    {
        protected override void Register() { }

        public virtual void AddBindings() { }

        public static void BindInstance(string name, object instance) => BindingUtils.BindInstance(name, instance);

        public static void BindMethod(string name, Delegate @delegate) => BindingUtils.BindInnerMethod(name, @delegate);

        public static bool RegisterSelector<Target>(string name, SelectorEntry<Target> entry)
        {
            if (entry is null || string.IsNullOrWhiteSpace(name))
                return false;

            if (!SelectorRegistry<Target>.Instance.Register(name, entry))
            {
                Warn($"selector @{name} already exists for {typeof(Target).Name}, skipped.");

                return false;
            }

            return true;
        }

        public static bool RegisterConditionChecker(Type type, Selector.ConditionChecker checker)
        {
            if (type is null || checker is null)
                return false;

            if (!Selector.ConditionCheckers.TryAdd(type, checker))
            {
                Warn($"condition checker for {type.FullName} already exists, skipped.");

                return false;
            }

            return true;
        }

        private static void Warn(string message) => TerraJS.Instance?.Logger.Warn($"[Plugin] {message}");
    }
}
