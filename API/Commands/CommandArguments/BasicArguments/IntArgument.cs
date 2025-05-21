using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    public class IntArgument(string name, int minValue = int.MinValue, int maxValue = int.MaxValue, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        /// <summary>
        /// Dont use this constructor, its just for ListArgument
        /// </summary>
        public IntArgument() : this("") { }

        private readonly int _minVal = minValue;

        private readonly int _maxVal = maxValue >= minValue ? maxValue : minValue;

        public override bool FromString(string content, out object value)
        {
            if (int.TryParse(content, out var res))
            {
                value = Math.Clamp(res, _minVal, _maxVal);

                return true;
            }

            value = "";

            return false;
        }

        public override bool FromStringWithoutClamp(string content, out object value)
        {
            if (int.TryParse(content, out var res))
            {
                value = res;

                return true;
            }

            value = "";

            return false;
        }

        public override string ToString()
        {
            var scope = "";

            if (_minVal != int.MinValue)
            {
                if (_maxVal != int.MaxValue)
                    scope = $"({_minVal}, {_maxVal})";
                else scope = $"({_minVal}, INTMAX)";
            }
            else if (_maxVal != int.MaxValue)
                scope = $"INTMIN, {_maxVal}";

            var ret = $"<{Name} : int{scope}>";

            return IsOptional ? $"[{ret}]" : ret;
        }

        public override Type InstanceType => typeof(int);

        public override bool InScope(object value) => value is int v && v >= _minVal && v <= _maxVal;

    }
}
