using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerraJS.Contents.Extensions
{
    public static class RegexExt
    {
        public static bool TryMatch(this Regex regex, string input, out Match match)
        {
            match = null;

            if (regex.IsMatch(input))
            {
                match = regex.Match(input);

                return true;
            }

            return false;
        }
    }
}
