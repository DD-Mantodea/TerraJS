using System;
using Microsoft.Xna.Framework;
using TerraJS.Contents.DataStructures;
using TerraJS.Contents.UI.Components.Containers;

namespace TerraJS.JSEngine.API.UIs.Components.Containers
{
    public class JsContainer : SizeContainer
    {
        public JsContainer() : base() { }

        public JsContainer(int width, int height) : base(width, height) { }

        public JsContainer(Vector2 size) : base(size) { }

        public Func<bool> VisibleGetter { get; set; }

        public override bool Visible => VisibleGetter?.Invoke() ?? true;
    }
}
