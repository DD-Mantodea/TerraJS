using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments.Selectors
{
    public class SelectorCondition(string variable, string value, string check)
    {
        private string _variable = variable;

        private string _value = value;

        private string _check = check;

        public bool Check(object obj)
        {
            var f = obj.GetType().GetField(_variable,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            var p = obj.GetType().GetProperty(_variable,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (f == null && p == null)
            {
                Main.NewText($"Field or Property not found: {_variable}");

                return false;
            }

            var type = f?.FieldType ?? p.PropertyType;

            if (Selector.ConditionCheckers.TryGetValue(f.FieldType.FullName, out var checker))
                return checker(type, _value, _check);

            Main.NewText($"Field or Property type not supported: {_variable}");

            return false;
        }

        public static bool CheckAll(object obj, List<SelectorCondition> conditions)
        {
            return conditions.All(c => c.Check(obj));
        }
    }
}
