using System;
using System.Linq;
using TerraJS.JSEngine.API.Commands.CommandArguments;
using TerraJS.JSEngine.API.Commands.Completion;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands
{
    public class CommandRegistry : ModTypeRegistry<TJSCommand, CommandRegistry>
    {
        public override string Namespace => "Commands";

        public CommandRegistry(string content, string name, string @namespace = "") : base(name == "" ? content + _tjsInstances.Where(c => c.Command == content).Count() : name, @namespace)
        {
            _content = content;
        }

        private readonly string _content;

        private readonly ArgumentGroup _argumentGroup = new();

        public CommandRegistry Description(GameCulture.CultureName gameCulture, string description)
        {
            if (IsEmpty) return this;

            TJSEngine.GlobalAPI.Translation.SetTranslation(GameCulture.FromCultureName(gameCulture), $"Commands.Description.{_builder.Name}", description);

            return this;
        }

        public CommandRegistry Next(CommandNode node)
        {
            if (IsEmpty) return this;

            _argumentGroup.Attach(node);

            return this;
        }

        public CommandRegistry Execute(Action<ArgumentInstanceGroup, CommandCaller> action)
        {
            if (IsEmpty) return this;

            _argumentGroup.Root.Execute(action);

            return this;
        }

        public override void Register(Mod mod)
        {
            if (IsEmpty || !_argumentGroup.HasAction) return;

            if (_tjsInstances.Exists(c => CommandAPI.CommandArgumentGroups[c.GetType().FullName] == _argumentGroup && c.Command == _content))
                return;

            var missing = _argumentGroup.MissingActions();

            if (missing.Count > 0)
                TJSEngine.GlobalAPI.Warn($"Command /{_content}: no action attached to {string.Join(" | ", missing)}, add .Execute(...) on them or the input will do nothing.");

            var cmdType = _builder.CreateType();

            var JSCommand = Activator.CreateInstance(cmdType) as TJSCommand;

            CommandAPI.CommandContents.Add(cmdType.FullName, _content);

            CommandAPI.CommandArgumentGroups.Add(cmdType.FullName, _argumentGroup);

            var action = _argumentGroup.Root.Action;

            if (action is not null)
                CommandAPI.CommandActions.Add(cmdType.FullName, action);

            mod.AddContent(JSCommand);

            _tjsInstances.Add(JSCommand);
        }
    }
}
