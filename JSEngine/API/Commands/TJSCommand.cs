using Microsoft.Xna.Framework;
using TerraJS.JSEngine.API.Commands.CommandArguments;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands
{
    [Autoload(false)]
    public abstract class TJSCommand : ModCommand
    {
        public override string Command { get => CommandAPI.CommandContents[GetType().FullName]; }

        public override string Description
        {
            get
            {
                var key = $"Commands.Description.{GetType().Name}";

                var translation = TJSEngine.GlobalAPI.Translation;

                var text = translation.GetTranslation(key, Language.ActiveCulture);

                if (text == key)
                    translation.DefaultLocalizedTexts.TryGetValue(key, out text);

                return string.IsNullOrEmpty(text) || text == key ? string.Empty : text;
            }
        }

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var argsGroup = CommandAPI.CommandArgumentGroups[GetType().FullName];

            var previous = CommandContext.Caller;

            CommandContext.Caller = caller;

            try
            {
                if (!argsGroup.TryParse(args, false, out var execution, out var expected) || execution.Action is null)
                {
                    caller.Reply(ErrorText(argsGroup, expected), Color.Red);

                    return;
                }

                var arguments = new ArgumentInstanceGroup();

                foreach (var pair in execution.Values)
                    arguments.TryAdd(pair.Key, pair.Value);

                execution.Action(arguments, caller);
            }
            finally
            {
                CommandContext.Caller = previous;
            }
        }

        private string ErrorText(ArgumentGroup argsGroup, string expected)
        {
            var usage = argsGroup.RenderUsage();

            var text = usage.Length == 0 ? $"{Localized("用法", "Usage")}: /{Command}" : $"{Localized("用法", "Usage")}: /{Command} {usage}";

            if (string.IsNullOrEmpty(expected))
                return text;

            var reason = Localized($"参数无效, 期望 {expected}", $"Invalid argument, expected {expected}");

            return $"{text} ({reason})";
        }

        private static string Localized(string chinese, string english) => Language.ActiveCulture.LegacyId == 7 ? chinese : english;

        public bool TryGetArgumentsText(string[] args, out string text)
        {
            var argsGroup = CommandAPI.CommandArgumentGroups[GetType().FullName];

            text = "";

            if (!argsGroup.TryParse(args, true, out var execution, out _))
                return false;

            var usage = argsGroup.RenderUsage(execution);

            text = usage.Length == 0 ? "" : " " + usage;

            return true;
        }

        public override string Usage => TryGetArgumentsText([], out var text) ? text : text;
    }
}
