using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.Completion
{
    public static class CommandSource
    {
        public static List<ModCommand> ChatCommands
        {
            get
            {
                var commands = new List<ModCommand>();

                if (typeof(CommandLoader).GetField("Commands", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) is not IDictionary<string, List<ModCommand>> all)
                    return commands;

                foreach (var list in all.Values)
                {
                    foreach (var command in list)
                    {
                        if (CommandLoader.Matches(command.Type, CommandType.Chat))
                            commands.Add(command);
                    }
                }

                return commands;
            }
        }
    }
}
