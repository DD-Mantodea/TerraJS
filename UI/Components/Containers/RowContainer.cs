using Microsoft.Xna.Framework;
using System;

namespace TerraJS.UI.Components.Containers
{
    public class RowContainer : Container
    {
        public RowContainer() { }

        public int ChildrenMargin = 0;

        public override void SetChildrenRelativePos()
        {
            _width = 0;

            foreach (var component in Children)
                if (component.Visible)
                    _height = Math.Max(component.Height + (int)component.RelativePosition.Y, Height);
            foreach (var component in Children)
            {
                if (!component.Visible) continue;
                component.RelativePosition.X = _width;
                _width += component.Width + ChildrenMargin;
            }
        }
    }
}
