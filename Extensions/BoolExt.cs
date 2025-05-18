using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Extensions
{
    public static class BoolExt
    {
        public static void Reverse(this ref bool @bool)
        {
            @bool = !@bool;
        }
    }
}
