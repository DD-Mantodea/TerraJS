using System;
using System.Collections.Generic;
using TerraJS.JSEngine.API.Commands.CommandArguments;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.Completion
{
    public sealed class CommandExecution
    {
        public Dictionary<string, object> Values = new();

        public Dictionary<CommandNode, object> NodeValues = new();

        public HashSet<CommandNode> Consumed = new();

        public object Last;

        public Action<ArgumentInstanceGroup, CommandCaller> Action;

        public CommandNode ActionNode;

        public CommandExecution Clone()
        {
            return new CommandExecution
            {
                Values = new Dictionary<string, object>(Values),
                NodeValues = new Dictionary<CommandNode, object>(NodeValues),
                Consumed = new HashSet<CommandNode>(Consumed),
                Last = Last,
                Action = Action,
                ActionNode = ActionNode,
            };
        }

        public void Record(CommandNode node, object value)
        {
            Consumed.Add(node);

            NodeValues[node] = value;

            if (node.Name.Length > 0)
                Values[node.Name] = value;

            if (node.Action is null)
                return;

            Action = node.Action;

            ActionNode = node;
        }
    }

    public sealed class CommandParsePath
    {
        public CommandNode Node;

        public CommandExecution Execution = new();

        public CommandParsePath Clone()
        {
            return new CommandParsePath
            {
                Node = Node,
                Execution = Execution.Clone(),
            };
        }
    }

    public abstract class CommandNode
    {
        public virtual string Name { get; set; } = string.Empty;

        public virtual bool IsOptional { get; set; }

        public readonly List<CommandNode> Children = [];

        public Action<ArgumentInstanceGroup, CommandCaller> Action;

        public virtual bool AcceptsToken => false;

        public virtual string UsageText => string.Empty;

        public virtual bool TryConsume(string token, CommandExecution execution, out string expected)
        {
            expected = string.Empty;

            return false;
        }

        public virtual IEnumerable<Suggestion> Suggest(CompletionContext context) => [];

        public virtual ArgumentState Validate(string token, CompletionContext template, out string expected)
        {
            expected = string.Empty;

            return ArgumentState.Valid;
        }

        public CommandNode Execute(Action<ArgumentInstanceGroup, CommandCaller> action)
        {
            Action = action;

            return this;
        }

        public CommandNode Next(CommandNode next)
        {
            if (next is not null)
                Children.Add(next);

            return this;
        }

        public void Consumers(List<CommandNode> result)
        {
            foreach (var child in Children)
            {
                if (child.AcceptsToken)
                    result.Add(child);

                if (child.IsOptional || !child.AcceptsToken)
                    child.Consumers(result);
            }
        }

        public void Leaves(List<CommandNode> result)
        {
            if (Children.Count == 0)
            {
                result.Add(this);

                return;
            }

            foreach (var child in Children)
                child.Leaves(result);
        }

        public bool HasAction => Action is not null || Children.Exists(child => child.HasAction);

        public bool CanEnd => Action is not null && (Children.Count == 0 || Children.Exists(child => child.IsOptional && child.CanEnd));

        public string MissingUsage
        {
            get
            {
                foreach (var child in Children)
                {
                    if (!child.IsOptional)
                        return child.UsageText;
                }

                foreach (var child in Children)
                {
                    var text = child.MissingUsage;

                    if (text.Length > 0)
                        return text;
                }

                return string.Empty;
            }
        }

        public virtual string GetUsageText(CommandExecution execution) => Colorize("A0A0A0", UsageText);

        protected static string Colorize(string color, string usage)
        {
            if (usage.Length > 2 && usage[0] == '[' && usage[^1] == ']')
                return "[" + $"[c/{color}:{usage[1..^1]}]" + "]";

            return $"[c/{color}:{usage}]";
        }

        public string RenderUsage() => Render(node => node.UsageText);

        public string RenderUsage(CommandExecution execution) => Render(node => node.GetUsageText(execution));

        public static LiteralNode Literal(string text, string name = "") => new(text, name);

        public static ArgumentNode Argument(CommandArgument argument) => new(argument);

        public static ArgumentNode Optional(CommandArgument argument) => new(argument) { IsOptional = true };

        private string Render(Func<CommandNode, string> format)
        {
            if (!AcceptsToken)
                return RenderAlternatives(format, false);

            var text = format(this);

            if (Children.Count == 0)
                return text;

            if (Children.Count == 1)
            {
                var rest = Children[0].Render(format);

                return rest.Length == 0 ? text : text + " " + rest;
            }

            var alternatives = RenderAlternatives(format, true);

            return alternatives.Length == 0 ? text : text + " " + alternatives;
        }

        private string RenderAlternatives(Func<CommandNode, string> format, bool wrap)
        {
            var parts = new List<string>();

            foreach (var child in Children)
            {
                var part = child.Render(format);

                if (part.Length > 0)
                    parts.Add(part);
            }

            if (parts.Count == 0)
                return string.Empty;

            if (parts.Count == 1)
                return parts[0];

            var text = string.Join(" | ", parts);

            return wrap ? "(" + text + ")" : text;
        }
    }

    public sealed class LiteralNode : CommandNode
    {
        public readonly string Text;

        public LiteralNode(string text, string name = "")
        {
            Text = text;

            Name = name;
        }

        public override bool AcceptsToken => true;

        public override string UsageText => IsOptional ? $"[{Text}]" : Text;

        public override string GetUsageText(CommandExecution execution) => Colorize(execution.Consumed.Contains(this) ? "F4F32B" : "A0A0A0", UsageText);

        public override bool TryConsume(string token, CommandExecution execution, out string expected)
        {
            expected = UsageText;

            if (!string.Equals(Text, token, StringComparison.OrdinalIgnoreCase))
                return false;

            execution.Record(this, Text);

            return true;
        }

        public override IEnumerable<Suggestion> Suggest(CompletionContext context)
        {
            if (!Text.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                yield break;

            yield return context.Value(Text, Text, null, context.Prefix.Length);
        }

        public override ArgumentState Validate(string token, CompletionContext template, out string expected)
        {
            expected = UsageText;

            if (string.Equals(Text, token, StringComparison.OrdinalIgnoreCase))
                return ArgumentState.Valid;

            return Text.StartsWith(token, StringComparison.OrdinalIgnoreCase) ? ArgumentState.Incomplete : ArgumentState.Invalid;
        }
    }

    public sealed class ArgumentNode : CommandNode
    {
        public readonly CommandArgument Argument;

        public ArgumentNode(CommandArgument argument)
        {
            Argument = argument;
        }

        public override string Name { get => Argument.Name; set => Argument.Name = value; }

        public override bool IsOptional { get => Argument.IsOptional; set => Argument.IsOptional = value; }

        public override bool AcceptsToken => true;

        public override string UsageText => Argument.ToString();

        public override string GetUsageText(CommandExecution execution)
        {
            if (!execution.Consumed.Contains(this))
                return Colorize("A0A0A0", UsageText);

            var value = execution.NodeValues.TryGetValue(this, out var parsed) ? parsed : null;

            return Colorize(Argument.InScope(value, null) ? "F4F32B" : "E74032", UsageText);
        }

        public override bool TryConsume(string token, CommandExecution execution, out string expected)
        {
            expected = UsageText;

            if (!Argument.TryParse(token, execution.Last, out var value, out var failure))
            {
                expected = failure;

                return false;
            }

            execution.Record(this, value);

            execution.Last = value;

            return true;
        }

        public override IEnumerable<Suggestion> Suggest(CompletionContext context) => Argument.Complete(context.With(Argument));

        public override ArgumentState Validate(string token, CompletionContext template, out string expected) => Argument.Validate(Token(template, token), out expected);

        private CompletionContext Token(CompletionContext template, string token)
        {
            return new CompletionContext
            {
                Info = template?.Info,
                Argument = Argument,
                Prefix = token,
                RawPrefix = token,
                Suffix = string.Empty,
                ArgumentIndex = template?.ArgumentIndex ?? -1,
                InBracket = false,
                InValue = false,
                AttributeName = string.Empty,
                AttributeOperator = string.Empty,
                ReplaceStart = template?.ReplaceStart ?? 0,
                ReplaceLength = template?.ReplaceLength ?? 0,
            };
        }
    }

    public static class CommandTree
    {
        public static List<CommandParsePath> Walk(CommandNode root, IReadOnlyList<string> tokens, CompletionContext template, out bool overrun)
        {
            var paths = new List<CommandParsePath> { new() { Node = root } };

            var finished = new List<CommandParsePath>();

            var expected = new List<string>();

            for (var i = 0; i < tokens.Count; i++)
            {
                var next = Advance(paths, tokens[i], true, template, finished, expected);

                if (next.Count == 0)
                {
                    overrun = true;

                    return finished.Count > 0 ? finished : paths;
                }

                paths = next;
            }

            overrun = false;

            return paths;
        }

        public static bool TryParse(CommandNode root, IReadOnlyList<string> tokens, bool allowIncomplete, out CommandExecution execution, out string expected)
        {
            var paths = new List<CommandParsePath> { new() { Node = root } };

            var finished = new List<CommandParsePath>();

            var failures = new List<string>();

            for (var i = 0; i < tokens.Count; i++)
            {
                var next = Advance(paths, tokens[i], false, null, finished, failures);

                if (next.Count == 0)
                {
                    if (finished.Count > 0)
                    {
                        execution = finished[0].Execution;
                        expected = string.Empty;

                        return true;
                    }

                    execution = null;
                    expected = failures.Count > 0 ? failures[0] : string.Empty;

                    return false;
                }

                paths = next;
            }

            foreach (var path in paths)
            {
                if (!allowIncomplete && !path.Node.CanEnd)
                    continue;

                execution = path.Execution;

                if (execution.Action is null && path.Node.Action is not null)
                {
                    execution.Action = path.Node.Action;

                    execution.ActionNode = path.Node;
                }

                expected = string.Empty;

                return true;
            }

            if (finished.Count > 0)
            {
                execution = finished[0].Execution;
                expected = string.Empty;

                return true;
            }

            execution = null;

            if (allowIncomplete)
            {
                expected = string.Empty;

                return true;
            }

            expected = paths.Count > 0 ? paths[0].Node.MissingUsage : string.Empty;

            return false;
        }

        private static List<CommandParsePath> Advance(List<CommandParsePath> paths, string token, bool explore, CompletionContext template, List<CommandParsePath> finished, List<string> expected)
        {
            var result = new List<CommandParsePath>();

            var visited = explore ? new HashSet<CommandNode>() : null;

            foreach (var path in paths)
            {
                var candidates = new List<CommandNode>();

                path.Node.Consumers(candidates);

                if (candidates.Count == 0)
                {
                    finished.Add(path);

                    continue;
                }

                foreach (var candidate in candidates)
                {
                    if (visited is not null && !visited.Add(candidate))
                        continue;

                    if (explore)
                    {
                        if (candidate.Validate(token, template, out var expectation) == ArgumentState.Invalid)
                        {
                            expected.Add(expectation);

                            continue;
                        }

                        var explored = path.Clone();

                        explored.Node = candidate;

                        result.Add(explored);

                        continue;
                    }

                    var next = path.Clone();

                    if (!candidate.TryConsume(token, next.Execution, out var failure))
                    {
                        expected.Add(failure);

                        continue;
                    }

                    next.Node = candidate;

                    result.Add(next);
                }
            }

            return result;
        }
    }
}
