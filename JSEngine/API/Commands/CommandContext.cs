using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands
{
    public static class CommandContext
    {
        public static CommandCaller Caller;

        public static Player Executor => Caller?.Player;
    }
}
