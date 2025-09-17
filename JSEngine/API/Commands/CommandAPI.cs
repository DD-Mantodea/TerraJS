using System;
using System.Collections.Generic;
using TerraJS.API.Commands.CommandArguments;
using TerraJS.JSEngine.API.Commands;
using Terraria.ModLoader;

namespace TerraJS.API.Commands
{
    public class CommandAPI : BaseAPI
    {
        public static Dictionary<string, string> CommandContents = [];

        public static Dictionary<string, ArgumentGroup> CommandArgumentGroups = [];

        public static Dictionary<string, Action<ArgumentInstanceGroup, CommandCaller>> CommandActions = [];

        public CommandRegistry CreateCommandRegistry(string content, string name = "", string @namespace = "") => new(content, name, @namespace);

        internal override void Unload()
        {

        }
    }
}
