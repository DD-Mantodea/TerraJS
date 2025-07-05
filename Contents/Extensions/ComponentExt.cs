using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;

namespace TerraJS.Contents.Extensions
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
