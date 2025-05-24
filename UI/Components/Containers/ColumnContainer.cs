using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.UI.Components.Containers
{
    public class ColumnContainer : Container
    {
        public ColumnContainer() { }

        public int ChildrenMargin = 0;

        public override void SetChildrenRelativePos()
        {
            _height = 0;

            foreach (var component in Children)
                if (component.Visible)
                    _width = Math.Max(component.Width + (int)component.RelativePosition.X, Width);
            foreach (var component in Children)
            {
                component.RelativePosition.Y = _height;
                if (!component.Visible)
                    continue;
                _height += component.Height + ChildrenMargin;
            }
        }
    }
}
