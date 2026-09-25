using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using TerraJS.JSEngine.API.CommandArguments.SelectorArguments;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria;
using Terraria.Localization;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments
{
    public abstract class SelectorArgument<Target> : CommandArgument
    {
        private static List<string> _conditionNames;

        protected SelectorArgument(string name, bool isOptional = false) : base(name, isOptional) { }

        public abstract SelectorRegistry<Target> Registry { get; }

        protected static Type TargetType => typeof(Target);

        protected static List<string> ConditionNames => _conditionNames ??= BuildConditionNames();

        protected static string Localized(string chinese, string english) => Language.ActiveCulture.LegacyId == 7 ? chinese : english;

        public virtual string NameHint => Localized("名称", "name");

        public virtual Vector2? PositionOf(Target target) => target is Entity entity ? entity.Center : null;

        public virtual bool TryResolveName(string text, out List<Target> targets)
        {
            targets = [];

            return false;
        }

        public virtual string Describe(string selectorName) => null;

        public virtual List<CompletionEntry> GetConditionValues(string conditionName)
        {
            var entries = new List<CompletionEntry>();

            if (string.Equals(conditionName, "sort", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var sort in new[] { "nearest", "furthest", "random", "arbitrary" })
                    entries.Add(new CompletionEntry(sort, sort));

                return entries;
            }

            var member = SelectorCondition.FindMember(TargetType, conditionName);

            if (member is null)
                return entries;

            var type = SelectorCondition.MemberType(member);

            if (type == typeof(bool))
            {
                entries.Add(new CompletionEntry("true", "true"));

                entries.Add(new CompletionEntry("false", "false"));
            }
            else if (type.IsEnum)
            {
                foreach (var enumName in Enum.GetNames(type))
                    entries.Add(new CompletionEntry(enumName, enumName));
            }

            return entries;
        }

        public override string ToString() => IsOptional ? $"[<{Name} : Selector | {TargetType.Name}>]" : $"<{Name} : Selector | {TargetType.Name}>";

        public override bool FromString(string content, object last, out object value) => TryParse(content, last, out value, out _);

        public override bool TryParse(string content, object last, out object value, out string expected)
        {
            value = null;

            expected = ToString();

            if (string.IsNullOrWhiteSpace(content))
                return false;

            if (!content.StartsWith('@'))
            {
                if (!TryResolveName(content, out var named) || named.Count == 0)
                {
                    expected = NameHint;

                    return false;
                }

                value = named;

                expected = string.Empty;

                return true;
            }

            if (!Selector.Split(content, out var identifier, out var body, out var terminated) || !terminated)
                return false;

            if (!Registry.TryFind(identifier, out var entry))
            {
                expected = "@" + identifier;

                return false;
            }

            var selector = CreateSelector();

            if (!ApplyConditions(selector, body, out expected))
                return false;

            if (!FinishOptions(selector, out expected))
                return false;

            var targets = Order(selector, entry, entry.Candidates(selector));

            if (targets.Count == 0)
            {
                expected = Localized("至少一个匹配的目标", "at least one matching target");

                return false;
            }

            value = targets;

            expected = string.Empty;

            return true;
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context?.Prefix ?? string.Empty;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (!token.StartsWith('@'))
            {
                if (context?.Info is { InBracket: true })
                    return ValidateSegment(token, out expected);

                if (TryResolveName(token, out var named) && named.Count > 0)
                    return ArgumentState.Valid;

                foreach (var entry in NameCompletions())
                {
                    if (entry.Id.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                        return ArgumentState.Incomplete;
                }

                return ArgumentState.Invalid;
            }

            if (!Selector.Split(token, out var identifier, out var body, out var terminated))
                return ArgumentState.Incomplete;

            if (!Registry.TryFind(identifier, out _))
            {
                foreach (var name in Registry.Names)
                {
                    if (name.StartsWith(identifier, StringComparison.OrdinalIgnoreCase))
                        return ArgumentState.Incomplete;
                }

                return ArgumentState.Invalid;
            }

            if (!terminated)
                return ArgumentState.Incomplete;

            if (body.Length == 0)
                return ArgumentState.Valid;

            foreach (var segment in body.Split(','))
            {
                var text = segment.Trim();

                if (text.Length == 0 || !SelectorCondition.TryParse(TargetType, text, out var condition) || !condition.IsValid)
                    return ArgumentState.Invalid;
            }

            return ArgumentState.Valid;
        }

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            var token = context.Prefix ?? string.Empty;

            if (context.InBracket && context.Info?.State == InputState.Selector)
            {
                if (!context.InValue)
                {
                    foreach (var name in ConditionNames)
                    {
                        if (name.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                            yield return context.Value(name, name, null, token.Length);
                    }

                    yield break;
                }

                foreach (var entry in GetConditionValues(context.AttributeName))
                {
                    if (!entry.Id.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                        continue;

                    yield return context.Value(entry.Id, entry.Display, entry.Description, token.Length);
                }

                yield break;
            }

            foreach (var entry in NameCompletions())
            {
                if (!entry.Id.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(entry.Id, entry.Display, entry.Description, token.Length);
            }

            foreach (var name in Registry.Names)
            {
                var selector = "@" + name;

                if (!selector.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(selector, selector, Describe(name), token.Length);

                if (string.Equals(selector, token, StringComparison.OrdinalIgnoreCase))
                    yield return context.Value(selector + "[", selector + "[", Describe(name), token.Length);
            }
        }

        public override List<string> GetCompletions(CommandInfo commandInfo)
        {
            return [.. Registry.Names.Select(name => "@" + name), .. NameCompletions().Select(entry => entry.Id)];
        }

        public override bool InScope(object value, object last) => value is List<Target> targets && targets.Count > 0;

        protected virtual IEnumerable<CompletionEntry> NameCompletions() => [];

        protected Selector CreateSelector()
        {
            var selector = new Selector(CommandContext.Executor);

            selector.PositionOf = target => target is Target typed ? PositionOf(typed) : null;

            return selector;
        }

        private static List<string> BuildConditionNames()
        {
            var names = new List<string>(Selector.VirtualNames);

            foreach (var field in TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Selector.Resolve(field.FieldType) is not null && !names.Contains(field.Name))
                    names.Add(field.Name);
            }

            foreach (var property in TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Selector.Resolve(property.PropertyType) is not null && !names.Contains(property.Name))
                    names.Add(property.Name);
            }

            names.Sort(StringComparer.OrdinalIgnoreCase);

            return names;
        }

        private static bool ApplyConditions(Selector selector, string body, out string expected)
        {
            expected = string.Empty;

            if (body.Length == 0)
                return true;

            foreach (var segment in body.Split(','))
            {
                var text = segment.Trim();

                if (text.Length == 0)
                    continue;

                if (!SelectorCondition.TryParse(TargetType, text, out var condition))
                {
                    expected = text;

                    return false;
                }

                if (!condition.IsValid)
                {
                    expected = text;

                    return false;
                }

                if (condition.IsOption)
                {
                    if (!condition.ApplyOption(selector, out expected))
                        return false;

                    continue;
                }

                selector.Conditions.Add(condition);
            }

            return true;
        }

        private static bool FinishOptions(Selector selector, out string expected)
        {
            expected = string.Empty;

            var options = selector.Options;

            if (options.OriginXSet || options.OriginYSet)
            {
                var origin = selector.DefaultOrigin;

                options.OriginSet = true;

                options.Origin = new Vector2(options.OriginXSet ? options.OriginX : origin.X, options.OriginYSet ? options.OriginY : origin.Y);
            }

            if (options.BoxSet && (!options.BoxWidthSet || !options.BoxHeightSet))
            {
                expected = Localized("dx 和 dy 必须同时给出", "dx and dy must be given together");

                return false;
            }

            return true;
        }

        private static List<Target> Order(Selector selector, SelectorEntry<Target> entry, List<Target> targets)
        {
            switch (selector.Options.SortSet ? selector.Options.Sort : entry.Sort)
            {
                case SelectorSort.Nearest:
                    targets = [.. targets.OrderBy(target => Distance(selector, target))];
                    break;
                case SelectorSort.Furthest:
                    targets = [.. targets.OrderByDescending(target => Distance(selector, target))];
                    break;
                case SelectorSort.Random:
                    Shuffle(targets);
                    break;
            }

            var limit = selector.Options.LimitSet ? selector.Options.Limit : entry.Limit;

            if (limit >= 0 && targets.Count > limit)
                targets = targets.GetRange(0, limit);

            return targets;
        }

        private static float Distance(Selector selector, Target target) => selector.TryGetPosition(target, out var position) ? Vector2.DistanceSquared(position, selector.Origin) : float.MaxValue;

        private static void Shuffle(List<Target> targets)
        {
            for (var i = targets.Count - 1; i > 0; i--)
            {
                var j = Main.rand.Next(i + 1);

                (targets[i], targets[j]) = (targets[j], targets[i]);
            }
        }

        private ArgumentState ValidateSegment(string token, out string expected)
        {
            expected = ToString();

            if (token.Length == 0 || token.EndsWith('=') || token.EndsWith('>') || token.EndsWith('<') || token.EndsWith('!'))
                return ArgumentState.Incomplete;

            if (SelectorCondition.TryParse(TargetType, token, out var condition))
                return condition.IsValid ? ArgumentState.Valid : ArgumentState.Invalid;

            foreach (var name in ConditionNames)
            {
                if (name.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                    return ArgumentState.Incomplete;
            }

            return ArgumentState.Invalid;
        }
    }
}
