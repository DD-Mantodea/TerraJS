using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;
using TerraJS.JSEngine.API.Items;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.EntityArguments
{
    public abstract class EntityArgument<T>(string name, bool isOptional) : CommandArgument(name, isOptional) where T : Entity
    {
        public static Regex RegExp = new Regex(@"^([a-zA-Z0-9_]+:[a-zA-Z0-9_]+)(?:\[((?:[a-zA-Z_][a-zA-Z0-9_]*=(?:""[^""]*""|[^,\]]+))(?:,(?:[a-zA-Z_][a-zA-Z0-9_]*=(?:""[^""]*""|[^,\]]+)))*)\])?$");

        public override List<string> GetCompletions(CommandInfo commandInfo)
        {
            var regex = new Regex(@"([a-zA-Z0-9_]+:[a-zA-Z0-9_]+)\[(.*)\]?");

            if (regex.TryMatch(commandInfo.CurrentParameter, out var match) && commandInfo.State == InputState.Entity)
            {
                var parts = match.Groups[2].Value.Split(',');

                var currentInput = parts.Select(s => s.Trim()).Last();

                var cursorPos = commandInfo.RelativeCursorPosition;

                var length = match.Groups[1].Length + 1;

                foreach (var part in parts)
                {
                    if (cursorPos > length)
                        length += part.Length;

                    if (cursorPos <= length)
                    {
                        currentInput = part;

                        break;
                    }

                    length++;
                }

                return DealStartWith([.. typeof(T).GetFields().Select(f => f.Name), .. typeof(T).GetProperties().Select(p => p.Name)], currentInput);
            }
            else
                return DealStartWith(GetAllIdentifiers(), commandInfo.CurrentParameter);
        }

        public abstract List<string> GetAllIdentifiers();

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            if (context.InBracket && context.Info.State == InputState.Entity)
            {
                if (!context.InValue)
                {
                    foreach (var member in GetMembers(typeof(T)))
                    {
                        if (!member.Key.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                            continue;

                        yield return context.Value(member.Key, member.Key, member.Value.Name, context.Prefix.Length);
                    }

                    yield break;
                }

                var type = FindMemberType(context.AttributeName);

                yield return context.Hint(type is null ? "<value>" : $"<{type.Name}>");

                yield break;
            }

            foreach (var entry in GetEntries())
            {
                if (!entry.Id.StartsWith(context.Prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(entry.Id, entry.Id, entry.Display, context.Prefix.Length);
            }
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = ToString();

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (!IdentifierPattern.IsMatch(token))
                return ArgumentState.Invalid;

            var bracket = token.IndexOf('[');

            var head = bracket < 0 ? token : token[..bracket];

            var parts = head.Split(':');

            if (parts.Length != 2 || parts[0].Length == 0 || parts[1].Length == 0)
                return ArgumentState.Incomplete;

            var exact = false;

            var prefixMatched = false;

            foreach (var entry in GetEntries())
            {
                if (string.Equals(entry.Id, head, StringComparison.OrdinalIgnoreCase))
                {
                    exact = true;

                    break;
                }

                if (entry.Id.StartsWith(head, StringComparison.OrdinalIgnoreCase))
                    prefixMatched = true;
            }

            if (!exact && !prefixMatched)
                return ArgumentState.Invalid;

            if (bracket < 0)
                return exact ? ArgumentState.Valid : ArgumentState.Incomplete;

            if (!token.EndsWith("]", StringComparison.Ordinal) || !exact)
                return ArgumentState.Incomplete;

            var members = GetMembers(typeof(T));

            foreach (var data in token[(bracket + 1)..^1].Split(','))
            {
                var assignment = data.Trim();

                if (assignment.Length == 0)
                    continue;

                var separator = assignment.IndexOf('=');

                if (separator <= 0)
                    return ArgumentState.Invalid;

                var name = assignment[..separator].Trim();

                var text = assignment[(separator + 1)..].Trim();

                if (!members.TryGetValue(name, out var memberType))
                    return ArgumentState.Invalid;

                if (memberType == typeof(int) && !int.TryParse(text, out _))
                    return ArgumentState.Invalid;
            }

            return ArgumentState.Valid;
        }

        private static readonly Regex IdentifierPattern = new(@"^[A-Za-z0-9_]*:?[A-Za-z0-9_]*(\[.*)?$");

        public virtual List<CompletionEntry> GetEntries()
        {
            var entries = new List<CompletionEntry>();

            foreach (var identifier in GetAllIdentifiers())
                entries.Add(new CompletionEntry(identifier, identifier));

            return entries;
        }

        private static readonly Dictionary<Type, Dictionary<string, Type>> MemberCache = [];

        private static Dictionary<string, Type> GetMembers(Type type)
        {
            if (MemberCache.TryGetValue(type, out var cached))
                return cached;

            var members = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var fullName = field.FieldType.FullName;

                if (fullName is not null && EntityAssignment.Assignments.ContainsKey(fullName))
                    members[field.Name] = field.FieldType;
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var fullName = property.PropertyType.FullName;

                if (fullName is not null && EntityAssignment.Assignments.ContainsKey(fullName))
                    members[property.Name] = property.PropertyType;
            }

            MemberCache[type] = members;

            return members;
        }

        private static Type FindMemberType(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return GetMembers(typeof(T)).TryGetValue(name, out var type) ? type : null;
        }

        public override string ToString() => IsOptional ? $"[<{Name} : {typeof(T).Name}>]" : $"<{Name} : {typeof(T).Name}>";
    }
}
