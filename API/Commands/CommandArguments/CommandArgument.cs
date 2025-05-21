using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Commands.CommandArguments
{
    public abstract class CommandArgument(string name, bool isOptional = false)
    {
        public CommandArgument() : this("") { }

        public string Name { get; set; } = name;

        public bool IsOptional { get; set; } = isOptional;

        public abstract bool FromString(string content, out object value);

        public virtual bool FromStringWithoutClamp(string content, out object value) => FromString(content, out value);

        public abstract override string ToString();

        public override int GetHashCode() => Name.GetHashCode();

        public virtual bool InScope(object value) => true;

        public bool SameType(CommandArgument arg) => GetType().FullName == arg.GetType().FullName;

        public static bool operator ==(CommandArgument c1, CommandArgument c2) => c1.Name == c2.Name;

        public static bool operator !=(CommandArgument c1, CommandArgument c2) => !(c1 == c2);

        public virtual Type InstanceType => typeof(object);

        public override bool Equals(object obj)
        {
            if (obj is null or not CommandArgument) return false;

            return this == (CommandArgument)obj;

            throw new NotImplementedException();
        }
    }
}
