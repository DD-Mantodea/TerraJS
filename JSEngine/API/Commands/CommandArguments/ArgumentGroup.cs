using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TerraJS.API.Commands.CommandArguments
{
    public class ArgumentGroup
    {
        private readonly List<CommandArgument> _arguments = [];

        public ReadOnlyCollection<CommandArgument> Arguments => _arguments.AsReadOnly();

        public ArgumentGroup Append(CommandArgument argument)
        {
            if (_arguments.Exists(arg => arg.Name == argument.Name))
                return this;

            _arguments.Add(argument);

            return this;
        }

        public bool Deserialize(string[] args, out ArgumentInstanceGroup group)
        {
            group = new();

            int argIndex = 0;

            object last = null;

            foreach (var arg in _arguments)
            {
                if (argIndex >= args.Length)
                {
                    if (!arg.IsOptional) return false;

                    continue;
                }

                if (arg.FromString(args[argIndex], last, out var value))
                {
                    group.Add(arg.Name, value);

                    last = value;

                    argIndex++;
                }
                else if (!arg.IsOptional)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Wont process value if value is not in scope.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool GetUseArguments(string[] args, out Dictionary<CommandArgument, object> arguments)
        {
            arguments = new();

            int argIndex = 0;

            object last = null;

            foreach (var arg in _arguments)
            {
                if (argIndex >= args.Length)
                    return true;

                if (arg.FromStringWithoutClamp(args[argIndex], last, out var value))
                {
                    arguments.Add(arg, value);

                    last = value;

                    argIndex++;
                }
                else if (!arg.IsOptional)
                    return false;
            }

            return true;
        }

        public static bool operator ==(ArgumentGroup g1, ArgumentGroup g2)
        {
            var args1 = g1._arguments;

            var args2 = g2._arguments;

            return args1.Count == args2.Count && args1.All(a1 => args2.Any(a2 => a2.SameType(a1) && a2.SameValue(a1)));
        }

        public static bool operator !=(ArgumentGroup g1, ArgumentGroup g2) => !(g1 == g2);

        public override bool Equals(object obj)
        {
            if (obj is null or not ArgumentGroup) return false;

            return this == (ArgumentGroup)obj;
        }
    }

    public class ArgumentInstanceGroup
    {
        private readonly Dictionary<string, object> _arguments = [];

        public bool TryAdd(string name, object arg) => _arguments.TryAdd(name, arg);

        public bool TryGet<T>(string name, out T arg)
        {
            arg = default;

            if (_arguments.TryGetValue(name, out var val))
            {
                arg = (T)val;
                return true;
            }

            return false;
        }

        public void Add(string name, object arg) => _arguments.Add(name, arg);

        public T Get<T>(string name) => (T)_arguments[name];

        public int GetInt(string name) => Get<int>(name);

        public string GetString(string name) => Get<string>(name);

        public bool GetBool(string name) => Get<bool>(name);

        public List<T> GetList<T>(string name) => Get<List<T>>(name);
    }
}
