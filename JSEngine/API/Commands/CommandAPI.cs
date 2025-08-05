using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jint.Runtime;
using Microsoft.Xna.Framework;
using TerraJS.API.Commands.CommandArguments;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Commands
{
    public class CommandAPI : BaseAPI
    {
        public static Dictionary<string, string> CommandContents = [];

        public static Dictionary<string, ArgumentGroup> CommandArgumentGroups = [];

        public static Dictionary<string, Action<ArgumentInstanceGroup, CommandCaller>> CommandActions = [];

        public CommandRegistry CreateCommandRegistry(string content, string name = "", string @namespace = "")
        {
            //只允许拉丁字母?

            if (string.IsNullOrWhiteSpace(content) || name.IsNullOrWhiteSpaceNotEmpty() || @namespace.IsNullOrWhiteSpaceNotEmpty())
            {
                return CommandRegistry.Empty;
            }

            var num = CommandRegistry._tjsInstances.Where(c => c.Command == content).Count();

            var commandName = $"TerraJS.Commands.{(@namespace == "" ? "" : @namespace + ".")}{(name == "" ? content + num : name)}";

            TypeBuilder builder = GlobalAPI._mb.DefineType(commandName, TypeAttributes.Public, typeof(TJSCommand));

            var registry = new CommandRegistry(builder, content);

            return registry;
        }

        internal override void Unload()
        {

        }
    }
}
