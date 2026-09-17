using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
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

        public override string ToString() => IsOptional ? $"[<{Name} : {typeof(T).Name}>]" : $"<{Name} : {typeof(T).Name}>";
    }
}
