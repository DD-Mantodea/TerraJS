using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TerraJS.Contents.Extensions
{
    public static class VectorExt
    {
        public static float GetRadian(this Vector2 v) => MathF.Atan2(v.Y, v.X);

        public static Vector2 Add(this Vector2 vec, float x, float y) => new(vec.X + x, vec.Y + y);

        public static Vector2 Sub(this Vector2 vec, float x, float y) => new(vec.X - x, vec.Y - y);

        public static Vector2 GetAngle(this float rad)
        {
            return new Vector2((float)Math.Cos((double)rad), (float)Math.Sin((double)rad));
        }

        public static Vector2 Multiply(this Vector2 vec, (int x, int y) turple)
        {
            return new(vec.X * turple.x, vec.Y * turple.y);
        }

        public static List<Vector2> Neighbors(this Vector2 v)
        {
            return
            [
                new(v.X, v.Y + 1),
                new(v.X - 1, v.Y + 1),
                new(v.X - 1, v.Y),
                new(v.X - 1, v.Y - 1),
                new(v.X, v.Y - 1),
                new(v.X + 1, v.Y - 1),
                new(v.X + 1, v.Y),
                new(v.X + 1, v.Y + 1),
            ];
        }
    }
}
