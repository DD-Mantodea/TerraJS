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
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Commands
{
    public class CommandAPI : BaseAPI
    {
        public static Dictionary<string, string> CommandContents = [];

        public static Dictionary<string, ArgumentGroup> CommandArgumentGroups = [];

        public static Dictionary<string, Action<ArgumentInstanceGroup, CommandCaller>> CommandActions = [];

        static CommandAPI()
        {
            MonoModHooks.Add(typeof(CommandLoader).GetMethod("HandleCommand", BindingFlags.Static | BindingFlags.NonPublic), HandleCommandHook);
        }

        internal static bool GetCommand(CommandCaller caller, string name, string[] args, out ModCommand mc)
        {
            string modName = null;
            if (name.Contains(':'))
            {
                var split = name.Split(':');
                modName = split[0];
                name = split[1];
            }

            var Commands = typeof(CommandLoader).GetField("Commands", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IDictionary<string, List<ModCommand>>;

            mc = null;

            if (!Commands.TryGetValue(name, out List<ModCommand> cmdList))
                return false;

            cmdList = cmdList.Where(c => CommandLoader.Matches(c.Type, caller.CommandType)).ToList();
            if (cmdList.Count == 0)
                return false;

            if (modName != null)
            {
                if (!ModLoader.TryGetMod(modName, out var mod))
                {
                    caller.Reply("Unknown Mod: " + modName, Color.Red);
                }
                else
                {
                    mc = cmdList.SingleOrDefault(c => c.Mod == mod);
                    if (mc == null)
                        caller.Reply("Mod: " + modName + " does not have a " + name + " command.", Color.Red);
                }
            }
            else if (cmdList.Count > 1)
            {
                if (cmdList.TrueForAll(cmd => cmd is TJSCommand))
                {
                    var cmds = cmdList.Where(cmd => CommandArgumentGroups[cmd.GetType().FullName].Deserialize(args, out _));

                    if (cmds.Count() > 1)
                    {
                        if (Language.ActiveCulture.LegacyId == 7)
                            caller.Reply("有多个命令 /" + name + "同时满足输入的参数", Color.Red);
                        else
                            caller.Reply("Multiple definitions of command /" + name + " and they all satisfy the arguments.", Color.Red);
                        foreach (var c in cmds)
                            caller.Reply(c.Mod.Name + ":" + c.Command, Color.Red);

                        return false;
                    }
                    else if (cmds.Count() == 0)
                    {
                        if (Language.ActiveCulture.LegacyId == 7)
                            caller.Reply("有多个命令 /" + name + "且都不满足输入的参数", Color.Red);
                        else
                            caller.Reply("Multiple definitions of command /" + name + " and they all do not satisfy the arguments.", Color.Red);

                        return false;
                    }
                    else
                        mc = cmds.First();
                }
                else
                {
                    caller.Reply("Multiple definitions of command /" + name + ". Try:", Color.Red);
                    foreach (var c in cmdList)
                        caller.Reply(c.Mod.Name + ":" + c.Command, Color.LawnGreen);
                }
            }
            else
            {
                mc = cmdList[0];
            }

            return true;
        }

        internal static bool HandleCommandHook(Func<string, CommandCaller, bool> orig, string input, CommandCaller caller)
        {
            int sep = input.IndexOf(" ");
            string name = (sep >= 0 ? input.Substring(0, sep) : input).ToLower();

            if (caller.CommandType != CommandType.Console)
            {
                if (name == "" || name[0] != '/')
                    return false;

                name = name.Substring(1);
            }

            var args = input.TrimEnd().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            args = args.Skip(1).ToArray();

            if (!GetCommand(caller, name, args, out ModCommand mc))
                return false;

            if (mc == null)//error in command name (multiple commands or missing mod etc)
                return true;

            if (!mc.IsCaseSensitive)
                input = input.ToLower();

            try
            {
                mc.Action(caller, input, args);
            }
            catch (Exception e)
            {
                var ue = e as UsageException;

                var color = typeof(UsageException).GetField("color", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ue);

                if (typeof(UsageException).GetField("msg", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ue) is string msg)
                    caller.Reply(msg, (Color)color);
                else
                    caller.Reply("Usage: " + mc.Usage, Color.Red);
            }
            return true;
        }

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
