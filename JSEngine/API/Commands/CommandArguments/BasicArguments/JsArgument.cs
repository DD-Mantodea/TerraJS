using System.Collections.Generic;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.BasicArguments
{
    public class JsArgument(string name, bool isOptional = false) : CommandArgument(name, isOptional)
    {
        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            using (new Logging.QuietExceptionHandle())
            {
                try
                {
                    value = TJSEngine.Engine.Evaluate(content).ToObject();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override List<string> GetCompletions(CommandInfo commandInfo) => [];

        public override string ToString() => IsOptional ? $"[<{Name} : jsExpression>]" : $"<{Name} : jsExpression>";
    }
}
