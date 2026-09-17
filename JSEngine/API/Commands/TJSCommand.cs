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

            if (!argsGroup.TryParse(args, false, out var execution, out _))
                return;

            if (execution.Action is null)
                return;

            var arguments = new ArgumentInstanceGroup();

            foreach (var pair in execution.Values)
                arguments.TryAdd(pair.Key, pair.Value);

            execution.Action(arguments, caller);
        }

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
