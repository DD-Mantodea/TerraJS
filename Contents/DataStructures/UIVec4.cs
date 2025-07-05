using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Contents.DataStructures
{
    public class UIVec4
    {
        public UIVec4(int val = 0)
        {
            X = val;
            Y = val;
            Z = val;
            W = val;
        }

        public UIVec4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public int X;

        public int Y;

        public int Z;

        public int W;

        public void Set(int val)
        {
            X = val; 
            Y = val; 
            Z = val; 
            W = val;
        }

        public void Set(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}
