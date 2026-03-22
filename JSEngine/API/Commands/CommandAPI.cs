using System;
using System.Collections.Generic;
using TerraJS.JSEngine.API.Commands.CommandArguments;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands
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
