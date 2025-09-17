using Microsoft.Xna.Framework;

namespace TerraJS.Contents.Utils
{
    public class RectangleUtils
    {
        public static Rectangle FromPoint(Point position, Point size) => new(position.X, position.Y, size.X, size.Y);

        public static Rectangle FromVector2(Vector2 position, Vector2 size) => new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
    }
}
