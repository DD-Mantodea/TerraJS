using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors
{
    public sealed class SelectorCondition
    {
        private static readonly Regex Pattern = new(@"^(?<name>[A-Za-z_][A-Za-z0-9_]*)(?<check>>=|<=|!=|=|>|<)(?<text>.+)$");

        private readonly Selector.ConditionChecker _checker;

        private readonly MemberInfo _member;

        private SelectorCondition(string variable, string check, string text, Selector.ConditionChecker checker, MemberInfo member, bool option, bool valid)
        {
            Variable = variable;

            Check = check;

            Text = text;

            IsOption = option;

            IsValid = valid;

            _checker = checker;

            _member = member;
        }

        public readonly string Variable;

        public readonly string Check;

        public readonly string Text;

        public readonly bool IsOption;

        public readonly bool IsValid;

        public static bool TryReadSort(string text, out SelectorSort sort)
        {
            switch (text?.Trim().ToLowerInvariant())
            {
                case "nearest":
                    sort = SelectorSort.Nearest;

                    return true;
                case "furthest":
                    sort = SelectorSort.Furthest;

                    return true;
                case "random":
                    sort = SelectorSort.Random;

                    return true;
                case "arbitrary":
                    sort = SelectorSort.None;

                    return true;
                default:
                    sort = SelectorSort.None;

                    return false;
            }
        }

        public static MemberInfo FindMember(Type targetType, string name)
        {
            if (targetType is null || string.IsNullOrEmpty(name))
                return null;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

            return (MemberInfo)targetType.GetField(name, flags) ?? targetType.GetProperty(name, flags);
        }

        public static Type MemberType(MemberInfo member) => member is FieldInfo field ? field.FieldType : ((PropertyInfo)member).PropertyType;

        public static bool TryParse(Type targetType, string segment, out SelectorCondition condition)
        {
            condition = null;

            if (string.IsNullOrWhiteSpace(segment))
                return false;

            var match = Pattern.Match(segment.Trim());

            if (!match.Success)
                return false;

            var variable = match.Groups["name"].Value;

            var check = match.Groups["check"].Value;

            var text = match.Groups["text"].Value.Trim();

            if (text.Length == 0)
                return false;

            if (Selector.IsOption(variable))
            {
                condition = new SelectorCondition(variable, check, text, null, null, true, ValidateOption(variable, check, text));

                return true;
            }

            if (Selector.IsFilter(variable))
            {
                condition = new SelectorCondition(variable, check, text, null, null, false, Selector.TryReadRange(text, out _, out _));

                return true;
            }

            var member = FindMember(targetType, variable);

            if (member is null)
            {
                condition = new SelectorCondition(variable, check, text, null, null, false, false);

                return true;
            }

            var type = MemberType(member);

            var checker = Selector.Resolve(type);

            condition = new SelectorCondition(variable, check, text, checker, member, false, checker is not null && ValidateOperand(type, check, text));

            return true;
        }

        public static bool CheckAllTarget(object target, Selector selector)
        {
            if (!selector.InBox(target))
                return false;

            return selector.Conditions.All(condition => condition.CheckTarget(target, selector));
        }

        public bool CheckTarget(object target, Selector selector)
        {
            if (!IsValid || IsOption || target is null)
                return false;

            if (Selector.IsFilter(Variable))
                return CheckFilter(target, selector);

            var value = _member is FieldInfo field ? field.GetValue(target) : ((PropertyInfo)_member).GetValue(target);

            return _checker(value, Text, Check);
        }

        public bool ApplyOption(Selector selector, out string expected)
        {
            expected = Variable;

            switch (Variable.ToLowerInvariant())
            {
                case "limit":
                    selector.Options.LimitSet = true;

                    selector.Options.Limit = int.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
                    break;
                case "sort":
                    TryReadSort(Text, out var sort);

                    selector.Options.SortSet = true;

                    selector.Options.Sort = sort;
                    break;
                case "x":
                    selector.Options.OriginXSet = true;

                    selector.Options.OriginX = (float)ReadTile(Text);
                    break;
                case "y":
                    selector.Options.OriginYSet = true;

                    selector.Options.OriginY = (float)ReadTile(Text);
                    break;
                case "dx":
                    selector.Options.BoxSet = true;

                    selector.Options.BoxWidthSet = true;

                    selector.Options.BoxSize.X = (float)ReadTile(Text);
                    break;
                case "dy":
                    selector.Options.BoxSet = true;

                    selector.Options.BoxHeightSet = true;

                    selector.Options.BoxSize.Y = (float)ReadTile(Text);
                    break;
                default:
                    return false;
            }

            expected = string.Empty;

            return true;
        }

        private static bool ValidateOption(string variable, string check, string text)
        {
            if (check != "=")
                return false;

            switch (variable.ToLowerInvariant())
            {
                case "limit":
                    return int.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, out var limit) && limit > 0;
                case "sort":
                    return TryReadSort(text, out _);
                case "x":
                case "y":
                    return Selector.TryReadNumber(text, out _);
                case "dx":
                case "dy":
                    return Selector.TryReadNumber(text, out var size) && size >= 0;
                default:
                    return false;
            }
        }

        private static bool ValidateOperand(Type type, string check, string text)
        {
            if (type == typeof(bool))
                return (check == "=" || check == "!=") && Selector.TryReadBool(text, out _);

            if (type == typeof(string))
                return check == "=" || check == "!=";

            if (type.IsEnum)
            {
                if (check != "=" && check != "!=")
                    return false;

                if (Enum.GetNames(type).Any(name => string.Equals(name, text, StringComparison.OrdinalIgnoreCase)))
                    return true;

                return Selector.TryReadNumber(text, out _);
            }

            if (Selector.ConditionCheckers.ContainsKey(type))
                return Selector.TryReadRange(text, out _, out _);

            return false;
        }

        private static double ReadTile(string text) => Selector.TryReadNumber(text, out var value) ? value * 16.0 : 0.0;

        private bool CheckFilter(object target, Selector selector)
        {
            if (!selector.TryGetPosition(target, out var center))
                return false;

            var distance = Vector2.Distance(center, selector.Origin) / 16f;

            switch (Variable.ToLowerInvariant())
            {
                case "r":
                    if (Check == "=" && Selector.TryReadNumber(Text, out var max))
                        return distance <= max;
                    break;
                case "rm":
                    if (Check == "=" && Selector.TryReadNumber(Text, out var min))
                        return distance >= min;
                    break;
            }

            return Selector.CheckNumber(distance, Text, Check);
        }
    }
}
