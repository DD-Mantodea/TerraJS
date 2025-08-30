using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Commands;
using TerraJS.API.Commands.CommandArguments;
using TerraJS.JSEngine.API.Commands;
using Terraria.ModLoader;

namespace TerraJS.Contents.Commands
{
    [Autoload(false)]
    public abstract class TJSCommand : ModCommand
    {
        public override string Command { get => CommandAPI.CommandContents[GetType().FullName]; }

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var argsGroup = CommandAPI.CommandArgumentGroups[GetType().FullName];

            var action = CommandAPI.CommandActions[GetType().FullName];

            if (argsGroup.Deserialize(args, out var arguments))
                action(arguments, caller);
        }

        public bool TryGetArgumentsText(string[] args, out string text)
        {
            var argsGroup = CommandAPI.CommandArgumentGroups[GetType().FullName];

            text = "";

            if (argsGroup.GetUseArguments(args, out var arguments))
            {
                object last = null;

                foreach (var arg in argsGroup.Arguments)
                {
                    if (arguments.ContainsKey(arg))
                    {
                        text += arg.InScope(arguments[arg], last) ? $" [c/F4F32B:{arg}]" : $" [c/E74032:{arg}]";

                        last = arguments[arg];
                    }
                    else
                        text += $" [c/A0A0A0:{arg}]";
                }

                return true;
            }

            return false;
        }
    }
}
