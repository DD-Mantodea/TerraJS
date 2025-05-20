using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using TerraJS.API.Commands.CommandArguments;
using Terraria.ModLoader;

namespace TerraJS.API.Commands
{
    public class CommandRegistry : Registry<TJSCommand>
    {
        public static CommandRegistry Empty => new() { isEmpty = true };

        public CommandRegistry() { }

        public CommandRegistry(TypeBuilder builder, string content)
        {
            _builder = builder;
            _content = content;
        }

        private Action<ArgumentInstanceGroup, CommandCaller> _action = null;

        private bool _end = false;

        private string _content;

        private ArgumentGroup _argumentGroup = new();

        public CommandRegistry NextArgument(CommandArgument argument)
        {
            if (isEmpty || _end) return this;

            _argumentGroup.Append(argument);

            return this;
        }

        public CommandRegistry Execute(Action<ArgumentInstanceGroup, CommandCaller> action)
        {
            if (isEmpty || _end) return this;

            _action = action;

            _end = true;

            return this;
        }

        public override void Register()
        {
            if (isEmpty || !_end) return;

            if (_tjsInstances.Exists(c => c.Command == _content))
                return;

            var cmdType = _builder.CreateType();

            var JSCommand = Activator.CreateInstance(cmdType) as TJSCommand;

            CommandAPI.CommandContents.Add(cmdType.FullName, _content);

            CommandAPI.CommandArgumentGroups.Add(cmdType.FullName, _argumentGroup);

            CommandAPI.CommandActions.Add(cmdType.FullName, _action);

            TJSMod.AddContent(JSCommand);

            _tjsInstances.Add(JSCommand);
        }
    }
}
