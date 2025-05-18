using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SilkyUIFramework;

namespace TerraJS.Extensions
{
    public static class SizeExt
    {
        public static Vector2 ToVec2(this Size size) => new Vector2(size.Width, size.Height);

        public static Vector3 ToVec3(this Size size) => new Vector3(size.Width, size.Height, 0);
    }
}
