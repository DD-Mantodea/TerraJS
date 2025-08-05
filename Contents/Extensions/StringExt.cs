using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Utils;

namespace TerraJS.Contents.Extensions
{
    public static class StringExt
    {
        public static string UpperFirst(this string s) => string.Concat(s[..1].ToUpper(), s[1..]);

        public static string LowerFirst(this string s) => string.Concat(s[..1].ToLower(), s[1..]); 

        public static IEnumerable<string> SplitListElements(this string input)
        {
            int depth = 0;
            StringBuilder current = new();

            foreach (char c in input)
            {
                switch (c)
                {
                    case '(':
                    case '[':
                    case '{':
                        depth++;
                        current.Append(c);
                        break;
                    case ')':
                    case ']':
                    case '}':
                        depth--;
                        current.Append(c);
                        break;
                    case ',' when depth == 0:
                        yield return current.ToString();
                        current.Clear();
                        break;
                    default:
                        current.Append(c);
                        break;
                }
            }

            if (current.Length > 0)
                yield return current.ToString();
        }

        public static bool IsNullOrWhiteSpaceNotEmpty(this string value)
        {
            if (value == null) 
                return true;

            if (value == "")
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }

        public static List<string> SplitWithCount(this string str, int count)
        {
            var ret = new List<string>();
            int i = 0;
            while (i < str.Length)
            {
                var sub = str.Substring(i, Math.Min(count, str.Length - i));

                ret.Add(sub);

                i += count;
            }

            return ret;
        }

        public static string NoColored(this string str)
        {
            var snippets = StringUtils.ParseMessage(str, Color.White);

            return string.Join(" ", snippets.ConvertAll(s => s.Text));
        }
    }
}
