using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using TerraJS.API.Commands.CommandArguments;
using TerraJS.API.Items;
using TerraJS.Extensions;
using Terraria.ModLoader;

namespace TerraJS.API.Commands
{
    public class CommandAPI
    {
        public static Dictionary<string, string> CommandContents = [];

        public static Dictionary<string, ArgumentGroup> CommandArgumentGroups = [];

        public static Dictionary<string, Action<ArgumentInstanceGroup, CommandCaller>> CommandActions = [];

        public CommandRegistry? CreateCommandRegistry(string content, string name = "", string @namespace = "")
        {
            var num = CommandRegistry._tjsInstances.Where(c => c.Command == content).Count();

            var commandName = $"TerraJS.Commands.{(@namespace == "" ? "" : @namespace + ".")}{(name == "" ? content + num : name)}";

            TypeBuilder builder = GlobalAPI._mb.DefineType(commandName, TypeAttributes.Public, typeof(TJSCommand));

            var registry = new CommandRegistry(builder, content);

            return registry;
        }
    }
}
