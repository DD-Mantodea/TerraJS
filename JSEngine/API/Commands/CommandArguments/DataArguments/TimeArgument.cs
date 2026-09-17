using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria;
using Terraria.GameContent.Creative;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.DataArguments
{
    public class TimeArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public static Regex RegExp = new(@"^(?:[01][0-9]|2[0-3]):([0-5][0-9])$");

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
