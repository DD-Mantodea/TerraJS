using Microsoft.Xna.Framework;
using Terraria;

namespace TerraJS.Contents.Utils
{
    public class UIUtils
    {
        public static Vector2 ScreenToWorld(Vector2 origin) => Vector2.Transform(origin, Main.UIScaleMatrix);

        public static Vector2 WorldToScreen(Vector2 origin) => Vector2.Transform(origin, Matrix.Invert(Main.UIScaleMatrix));
    }
}
