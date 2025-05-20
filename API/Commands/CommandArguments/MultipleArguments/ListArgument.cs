using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerraJS.API.Commands.CommandArguments.MultipleArguments
{
    public class ListArgument<T>(string name, int minLength = 0, int maxLength = int.MaxValue, bool isOptional = false) : CommandArgument(name, isOptional) where T : CommandArgument, new()
    {
        private T _argumentInstance = new T();

        private int _minLength = minLength >= 0 ? minLength : 0;

        private int _maxLength = maxLength >= minLength ? maxLength : minLength;

        public override bool FromString(string content, out object value)
        {
            value = null;

            if (!Pattern.IsMatch(content))
                return false;

            var list = new List<object>();

            var args = Pattern.Match(content).Groups[1].Value.Replace(" ", "").Split(",");

            foreach (var arg in args)
            {
                if (_argumentInstance.FromString(arg, out var val))
                {
                    list.Add(val);

                    continue;
                }
                
                return false;
            }

            while (list.Count < _minLength) 
                list.Add(null);

            while (list.Count > _maxLength) 
                list.RemoveAt(list.Count - 1);

            value = list;

            return true;
        }

        public override bool FromStringWithoutClamp(string content, out object value)
        {
            value = null;

            if (!Pattern.IsMatch(content))
                return false;

            var list = new List<object>();

            var args = Pattern.Match(content).Groups[1].Value.Replace(" ", "").Split(",");

            foreach (var arg in args)
            {
                if (_argumentInstance.FromString(arg, out var val))
                {
                    list.Add(val);

                    continue;
                }

                return false;
            }

            value = list;

            return true;
        }

        public override string ToString()
        {
            var scope = "";

            if (_maxLength != int.MaxValue)
                scope = $"length({_minLength}, {_maxLength})";
            else scope = $"length({_minLength}, INTMAX)";

            var content = $"<{Name} : list<{nameof(T)}> {scope}>";

            return IsOptional ? $"[{content}]" : content;
        }

        public override bool InScope(object value)
        {
            return value is List<object> list && list.Count >= _minLength && list.Count <= _maxLength;
        }

        private static Regex Pattern => new("\\[(.*)\\]");
    }
}
