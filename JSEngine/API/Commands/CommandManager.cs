using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TerraJS.DetectorJS;
using TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments;
using TerraJS.JSEngine.API.Commands.CommandArguments.DataArguments;
using TerraJS.JSEngine.API.Commands.CommandArguments.EntityArguments;
using TerraJS.JSEngine.API.Commands.CommandArguments.SelectorArguments;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands
{
    public class CommandManager
    {
        public static CommandAPI Cmd => TJSEngine.GlobalAPI.Command;

        public static void RegisterCommands()
        {
            JsCommands();

            PlayerCommands();

            UtilCommands();
        }

        public static void JsCommands()
        {
            Cmd.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "reload"))
                .Execute((_, _) => TerraJS.Reload())
                .Register();

            Cmd.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "detect"))
                .Execute((_, _) =>
                {
                    var thread = new Thread(Detector.Detect);

                    thread.Start();
                })
                .Register();

            Cmd.CreateCommandRegistry("exec")
                .NextArgument(new StringArgument("code"))
                .Execute((g, _) =>
                {
                    var code = g.GetString("code");

                    try
                    {
                        TJSEngine.Engine.Execute(code);
                    }
                    catch
                    {

                    }
                })
                .Register();
        }

        public static void PlayerCommands()
        {
            Cmd.CreateCommandRegistry("tp")
                .NextArgument(new PlayersArgument("players"))
                .NextArgument(new IntArgument("x"))
                .NextArgument(new IntArgument("y"))
                .Execute((g, _) =>
                {
                    var x = g.GetInt("x");

                    var y = g.GetInt("y");

                    g.Get<List<Player>>("players").ForEach(p => p.position = new(x, y));
                })
                .Register();

            Cmd.CreateCommandRegistry("give")
                .NextArgument(new PlayersArgument("players"))
                .NextArgument(new ItemArgument("item"))
                .NextArgument(new IntArgument("stack", 0, isOptional: true))
                .Execute((g, _) =>
                {
                    var players = g.Get<List<Player>>("players");

                    var item = g.Get<Item>("item");

                    if (!g.TryGet<int>("stack", out var stack))
                        stack = 1;

                    foreach (var p in players)
                        p.QuickSpawnClonedItemDirect(new CommandEntitySource(), item, stack);
                })
                .Register();

            Cmd.CreateCommandRegistry("prefix")
                .NextArgument(new PlayersArgument("players"))
                .NextArgument(new PrefixArgument("prefix"))
                .Execute((g, _) =>
                {
                    var players = g.Get<List<Player>>("players");

                    var prefixID = g.GetInt("prefix");

                    foreach (var plr in players)
                    {
                        plr.HeldItem.ResetPrefix();

                        plr.HeldItem.Prefix(prefixID);
                    }
                })
                .Register();
        }

        public static void UtilCommands()
        {
            Cmd.CreateCommandRegistry("time")
                .NextArgument(new ConstantArgument("feature", "set"))
                .NextArgument(new TimeArgument("time"))
                .Execute((g, _) =>
                {
                    var time = g.Get<Time>("time");

                    Main.SkipToTime(time, time.IsDayTime);
                })
                .Register();

            Cmd.CreateCommandRegistry("time")
                .NextArgument(new ConstantArgument("feature", "set"))
                .NextArgument(new EnumArgument("preset", typeof(TimePreset)))
                .Execute((g, _) =>
                {
                    var time = g.Get<TimePreset>("preset");

                    switch (time)
                    {
                        case TimePreset.Dawn:
                            Main.SkipToTime(0, true);

                            break;
                        case TimePreset.Noon:
                            Main.SkipToTime(27000, true);

                            break;
                        case TimePreset.Dusk:
                            Main.SkipToTime(0, true);

                            break;
                        case TimePreset.Midnight:
                            Main.SkipToTime(16200, false);

                            break;
                    }
                })
                .Register();

            Cmd.CreateCommandRegistry("spawnpoint")
                .Execute((_, _) =>
                {
                    var plr = Main.LocalPlayer;

                    plr.SpawnX = (int)plr.position.X / 16;

                    plr.SpawnY = (int)plr.position.Y / 16;
                })
                .Register();
        }
    }
}
