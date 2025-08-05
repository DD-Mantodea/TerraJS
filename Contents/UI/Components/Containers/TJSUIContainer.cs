using System;
using Microsoft.Xna.Framework;
using TerraJS.Contents.DataStructures;

namespace TerraJS.Contents.UI.Components.Containers
{
    public class TJSUIContainer : SizeContainer
    {
        public TJSUIContainer() : base() { }

        public TJSUIContainer(int width, int height) : base(width, height) { }

        public TJSUIContainer(Vector2 size) : base(size) { }

        public Func<bool> VisibleGetter { get; set; }

        public override bool Visible => VisibleGetter?.Invoke() ?? true;
    }
}
