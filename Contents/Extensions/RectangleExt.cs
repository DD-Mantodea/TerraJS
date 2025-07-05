using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TerraJS.Contents.Extensions
{
    public static class RectangleExt
    {
        public static Rectangle StretchDown(this Rectangle rect, int Height)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + Height);
        }

        public static Rectangle Divide(this Rectangle rect, int num)
        {
            return new Rectangle(rect.X / num, rect.Y / num, rect.Width / num, rect.Height / num);
        }
    }
}
