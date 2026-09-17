using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors;
using Terraria;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments
{
    public abstract class Selector(Entity sender)
    {
        public Entity Sender = sender;

        public List<SelectorCondition> Conditions = [];

        public delegate bool ConditionChecker(object target, string value, string check);

        public static Dictionary<string, ConditionChecker> ConditionCheckers = [];

        public static Regex RegExp = new Regex(@"^@([a-zA-Z][A-Za-z0-9]*)(?:\[((?:(?:(?![0-9]+$)[a-zA-Z0-9]+)(?:(?:>|<|=|>=|<=)[^,(?:?!=)\]]+)?)(?:,(?:(?:(?![0-9]+$)[a-zA-Z][a-zA-Z0-9]*)(?:(?:>|<|=|>=|<=)[^,\]]+)?))*)\])?$");

        static Selector()
        {
            ConditionCheckers.Add(typeof(int).FullName, (target, value, check) =>
            {
                var targetVal = (int)target;

                if (!int.TryParse(value, out var val))
                    return false;

                return check switch
                {
                    ">" => targetVal > val,
                    "<" => targetVal < val,
                    "=" => targetVal == val,
                    ">=" => targetVal >= val,
                    "<=" => targetVal <= val,
                    _ => false,
                };
            });

            ConditionCheckers.Add(typeof(string).FullName, (target, value, check) =>
            {
                var targetVal = (string)target;

                return check switch
                {
                    "=" => targetVal == value,
                    "!=" => targetVal != value,
                    _ => false,
                };
            });
        }
    }

    public interface ISelector<TSelector, Target> where TSelector : Selector where Target : Entity
    {
        public abstract static Dictionary<string, Func<TSelector, List<Target>>> Selectors { get; }
    }
}
