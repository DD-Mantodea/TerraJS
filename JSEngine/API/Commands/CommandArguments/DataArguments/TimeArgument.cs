using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria;
using Terraria.GameContent.Creative;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.DataArguments
{
    public class TimeArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public static Regex RegExp = new(@"^([01][0-9]|2[0-3]):([0-5][0-9])$");

        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (RegExp.TryMatch(content, out var match))
            {
                var hour = int.Parse(match.Groups[1].Value);

                var minute = int.Parse(match.Groups[2].Value);

                value = new Time(hour, minute);
            }

            return value != null;
        }

        public override List<string> GetCompletions(CommandInfo commandInfo) => [];

        public override IEnumerable<Suggestion> Complete(CompletionContext context)
        {
            var prefix = context.Prefix;

            if (prefix.Length == 2 && int.TryParse(prefix, out var hour) && hour <= 23)
            {
                var value = prefix + ":00";

                yield return context.Value(value, value, null, prefix.Length);

                yield break;
            }

            foreach (var value in Presets)
            {
                if (!value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                yield return context.Value(value, value, null, prefix.Length);
            }

            yield return context.Hint("HH:MM");
        }

        public override ArgumentState Validate(CompletionContext context, out string expected)
        {
            expected = "HH:MM";

            var token = context.Prefix;

            if (token.Length == 0)
                return ArgumentState.Incomplete;

            if (TryParse(token, null, out _, out _))
                return ArgumentState.Valid;

            return TimePattern.IsMatch(token) ? ArgumentState.Incomplete : ArgumentState.Invalid;
        }

        private static readonly Regex TimePattern = new(@"^([01]?\d|2[0-3])?(:\d{0,2})?$");

        private static readonly string[] Presets = ["00:00", "06:00", "12:00", "18:00"];

        public override string ToString() => IsOptional ? $"[<{Name} : Time>]" : $"<{Name} : Time>";
    }

    public record struct Time(int Hour, int Minute) : IEquatable<Time>
    {
        public readonly bool IsDayTime => Hour <= 15;

        public readonly bool Equals(Time other) => other.Hour == Hour && other.Minute == Minute;

        public static implicit operator int(Time time) => (time.Hour * 60 + time.Minute) * 60;

        public static implicit operator Time(int ticks) => new(ticks / 3600, (ticks / 60) % 60);
    }

    public enum TimePreset
    {
        Dawn,
        Noon,
        Dusk,
        Midnight,
    }
}
