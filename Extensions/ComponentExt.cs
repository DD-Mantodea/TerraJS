using TerraJS.UI.Components;
using TerraJS.UI.Components.Containers;

namespace TerraJS.Extensions
{
    public static class ComponentExt
    {
        public static T Join<T>(this T component, Container container) where T : Component
        {
            container.RegisterChild(component);

            return component;
        }
    }
}
