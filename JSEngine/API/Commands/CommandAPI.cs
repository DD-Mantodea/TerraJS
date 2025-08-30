using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Jint.Runtime;
using Microsoft.Xna.Framework;
using TerraJS.API.Commands.CommandArguments;
using TerraJS.Contents.Commands;
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

        public CommandRegistry CreateCommandRegistry(string content, string name = "", string @namespace = "") => new(content, name, @namespace);

        internal override void Unload()
        {

        }
    }
}
