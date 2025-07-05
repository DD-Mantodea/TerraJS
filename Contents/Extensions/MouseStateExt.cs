using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TerraJS.Contents.Extensions
{
    public static class MouseStateExt
    {
        public static Vector2 Position(this MouseState state) => new(state.X, state.Y);
    }
}
