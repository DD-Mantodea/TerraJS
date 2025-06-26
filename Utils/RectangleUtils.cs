using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TerraJS.Utils
{
    public class RectangleUtils
    {
        public static Rectangle FormPoint(Point position, Point size) => new(position.X, position.Y, size.X, size.Y);

        public static Rectangle FormVector2(Vector2 position, Vector2 size) => new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
    }
}
