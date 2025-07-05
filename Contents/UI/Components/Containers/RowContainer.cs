using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using TerraJS.Contents.UI.Components;

namespace TerraJS.Contents.UI.Components.Containers
{
    public class RowContainer : Container
    {
        public RowContainer() { }

        public int ChildrenMargin = 0;

        public override void SetChildRelativePos(Component child)
        {
            _height = Math.Max(child.Height + (int)child.RelativePosition.Y, Height);

            child.RelativePosition.X = _width;

            _width += child.Width + ChildrenMargin;

            base.SetChildRelativePos(child);
        }

        public override void UpdateChildren(GameTime gameTime)
        {
            _width = 0;

            base.UpdateChildren(gameTime);
        }
    }
}
