using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraJS.Contents.UI.Components;

namespace TerraJS.Contents.UI.Components.Containers
{
    public class ColumnContainer : Container
    {
        public ColumnContainer() { }

        public int ChildrenMargin = 0;

        public override void SetChildRelativePos(Component child)
        {
            _width = Math.Max(child.Width + (int)child.RelativePosition.X, Width);

            child.RelativePosition.Y = _height;

            _height += child.Height + ChildrenMargin;

            base.SetChildRelativePos(child);
        }

        public override void UpdateChildren(GameTime gameTime)
        {
            _height = 0;

            base.UpdateChildren(gameTime);
        }
    }
}
