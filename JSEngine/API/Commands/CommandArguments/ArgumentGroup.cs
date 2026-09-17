using System;
using System.Collections.Generic;
using System.Linq;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.CommandArguments
{
    public class ArgumentGroup
    {
        public ArgumentGroup()
        {
            Root = new GroupNode();
        }

        public CommandNode Root { get; }

        private sealed class GroupNode : CommandNode
        {
        }

        public bool HasAction => Root.HasAction;

        public void Attach(CommandNode node) => Root.Next(node);

        public List<string> MissingActions() => MissingActions(Root, string.Empty);

        private static List<string> MissingActions(CommandNode node, string path)
        {
            var result = new List<string>();

            if (node.AcceptsToken)
                path = path.Length == 0 ? node.UsageText : path + " " + node.UsageText;

            if (node.Action is null && node.Children.All(child => child.IsOptional))
                result.Add(path);

            foreach (var child in node.Children)
                result.AddRange(MissingActions(child, path));

            return result;
        }

        public bool TryParse(string[] args, bool allowIncomplete, out CommandExecution execution, out string expected) => CommandTree.TryParse(Root, args, allowIncomplete, out execution, out expected);

        public string RenderUsage() => Root.RenderUsage();

        public string RenderUsage(CommandExecution execution) => Root.RenderUsage(execution);

        public bool Deserialize(string[] args, out ArgumentInstanceGroup group)
        {
            group = new();

            if (!TryParse(args, false, out var execution, out _))
                return false;

            foreach (var pair in execution.Values)
                group.TryAdd(pair.Key, pair.Value);

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

            if (!TryParse(args, true, out var execution, out _))
                return false;

            foreach (var pair in execution.NodeValues)
            {
                if (pair.Key is ArgumentNode node)
                    arguments[node.Argument] = pair.Value;
            }

            return true;
        }

        public static bool operator ==(ArgumentGroup g1, ArgumentGroup g2) => g1.RenderUsage() == g2.RenderUsage();

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
