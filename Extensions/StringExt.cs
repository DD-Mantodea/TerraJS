using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Extensions
{
    public static class StringExt
    {
        public static string UpperFirst(this string s)  => string.Concat(s[..1].ToUpper(), s[1..]);
    }
}
