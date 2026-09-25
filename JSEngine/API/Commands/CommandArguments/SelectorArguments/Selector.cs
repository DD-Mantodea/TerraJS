using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors;
using Terraria;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments
{
    public class Selector(object sender)
    {
        public object Sender = sender;

        public List<SelectorCondition> Conditions = [];

        public SelectorOptions Options = new();

        public Func<object, Vector2?> PositionOf;

        public delegate bool ConditionChecker(object value, string text, string check);

        public static Dictionary<Type, ConditionChecker> ConditionCheckers = [];

        public static List<string> FilterNames = ["distance", "r", "rm"];

        public static List<string> OptionNames = ["x", "y", "dx", "dy", "limit", "sort"];

        public static List<string> VirtualNames = [.. FilterNames, .. OptionNames];

        public Vector2 DefaultOrigin => Sender is { } current && TryGetPosition(current, out var position) ? position : new Vector2(Main.spawnTileX * 16f + 8f, Main.spawnTileY * 16f + 8f);

        public Vector2 Origin => Options.OriginSet ? Options.Origin : DefaultOrigin;

        public bool TryGetPosition(object target, out Vector2 position)
        {
            position = default;

            if (target is null)
                return false;

            if (PositionOf?.Invoke(target) is { } custom)
            {
                position = custom;

                return true;
            }

            if (target is Entity entity)
            {
                position = entity.Center;

                return true;
            }

            return false;
        }

        public bool InBox(object target)
        {
            if (!Options.BoxSet)
                return true;

            if (!TryGetPosition(target, out var center))
                return false;

            var min = Origin;

            var max = min + Options.BoxSize;

            return center.X >= min.X && center.X <= max.X && center.Y >= min.Y && center.Y <= max.Y;
        }

        public static bool IsFilter(string name) => FilterNames.Contains(name, StringComparer.OrdinalIgnoreCase);

        public static bool IsOption(string name) => OptionNames.Contains(name, StringComparer.OrdinalIgnoreCase);

        public static bool IsVirtual(string name) => IsFilter(name) || IsOption(name);

        public static bool IsIdentifier(string text) => text.Length > 0 && text.All(c => char.IsLetterOrDigit(c) || c == '_');

        public static bool Split(string text, out string identifier, out string body, out bool terminated)
        {
            identifier = string.Empty;

            body = string.Empty;

            terminated = true;

            if (string.IsNullOrEmpty(text) || text[0] != '@')
                return false;

            var bracket = text.IndexOf('[');

            if (bracket < 0)
            {
                identifier = text[1..];

                return IsIdentifier(identifier);
            }

            identifier = text[1..bracket];

            if (!IsIdentifier(identifier))
                return false;

            if (text.EndsWith(']'))
                body = text[(bracket + 1)..^1];
            else
            {
                body = text[(bracket + 1)..];

                terminated = false;
            }

            return true;
        }

        public static ConditionChecker Resolve(Type type)
        {
            if (type is null)
                return null;

            if (ConditionCheckers.TryGetValue(type, out var checker))
                return checker;

            return type.IsEnum ? CheckEnum : null;
        }

        public static bool TryReadNumber(string text, out double value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            return double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryReadRange(string text, out double low, out double high)
        {
            low = double.NegativeInfinity;

            high = double.PositiveInfinity;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            var value = text.Trim();

            var dots = value.IndexOf("..", StringComparison.Ordinal);

            if (dots < 0)
            {
                if (!TryReadNumber(value, out var single))
                    return false;

                low = single;

                high = single;

                return true;
            }

            var left = value[..dots];

            var right = value[(dots + 2)..];

            if (left.Length > 0 && !TryReadNumber(left, out low))
                return false;

            if (right.Length > 0 && !TryReadNumber(right, out high))
                return false;

            return true;
        }

        public static bool TryReadBool(string text, out bool value)
        {
            value = false;

            switch (text?.Trim().ToLowerInvariant())
            {
                case "true":
                case "1":
                case "yes":
                case "on":
                    value = true;

                    return true;
                case "false":
                case "0":
                case "no":
                case "off":
                    value = false;

                    return true;
                default:
                    return false;
            }
        }

        public static bool CheckNumber(object value, string text, string check)
        {
            if (value is null || !TryReadRange(text, out var low, out var high))
                return false;

            var number = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            return check switch
            {
                "=" => number >= low && number <= high,
                "!=" => number < low || number > high,
                ">" => number > high,
                ">=" => number >= low,
                "<" => number < low,
                "<=" => number <= high,
                _ => false,
            };
        }

        public static bool CheckBool(object value, string text, string check)
        {
            if (value is not bool flag || !TryReadBool(text, out var expected))
                return false;

            return check switch
            {
                "=" => flag == expected,
                "!=" => flag != expected,
                _ => false,
            };
        }

        public static bool CheckString(object value, string text, string check)
        {
            if (value is not string current)
                return false;

            return check switch
            {
                "=" => string.Equals(current, text, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(current, text, StringComparison.OrdinalIgnoreCase),
                _ => false,
            };
        }

        public static bool CheckEnum(object value, string text, string check)
        {
            if (value is null)
                return false;

            var equal = string.Equals(value.ToString(), text?.Trim(), StringComparison.OrdinalIgnoreCase);

            if (!equal && TryReadNumber(text, out var number))
                equal = Convert.ToDouble(value, CultureInfo.InvariantCulture) == number;

            return check switch
            {
                "=" => equal,
                "!=" => !equal,
                _ => false,
            };
        }

        static Selector()
        {
            ConditionCheckers[typeof(int)] = CheckNumber;

            ConditionCheckers[typeof(long)] = CheckNumber;

            ConditionCheckers[typeof(short)] = CheckNumber;

            ConditionCheckers[typeof(byte)] = CheckNumber;

            ConditionCheckers[typeof(sbyte)] = CheckNumber;

            ConditionCheckers[typeof(uint)] = CheckNumber;

            ConditionCheckers[typeof(ulong)] = CheckNumber;

            ConditionCheckers[typeof(ushort)] = CheckNumber;

            ConditionCheckers[typeof(float)] = CheckNumber;

            ConditionCheckers[typeof(double)] = CheckNumber;

            ConditionCheckers[typeof(decimal)] = CheckNumber;

            ConditionCheckers[typeof(bool)] = CheckBool;

            ConditionCheckers[typeof(string)] = CheckString;
        }
    }
}
