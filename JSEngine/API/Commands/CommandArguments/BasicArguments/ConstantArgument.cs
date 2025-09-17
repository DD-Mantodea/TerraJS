using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerraJS.Contents.Utils;

namespace TerraJS.API.Commands.CommandArguments.BasicArguments
{
    public class ConstantArgument(string name, string content, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public readonly string _content = content;   

        public static StringArgument New(string name, dynamic options = default)
            => new(name, OptionUtils.GetOption<bool>(options, "isOptional", false));


        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (content != _content)
                return false;

            value = content;

            return true;
        }

        public override bool FromStringWithoutClamp(string content, object last, out object value)
        {
            value = content;

            return true;
        }

        public override bool SameValue(CommandArgument arg)
        {
            var cons = arg as ConstantArgument;

            return cons._content == _content;
        }

        public override Type InstanceType => typeof(string);

        public override string ToString() => IsOptional ? $"[<{Name} = {_content}>]" : $"<{Name} = {_content}>";

        public override List<string> GetCompletions() => [_content];

        public override bool InScope(object value, object last) => value is string str && str == _content;

        private static Regex Pattern => new("\"(.*)\"");
    }
}
