using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
using Terraria;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorMethod(MethodInfo method) : DetectorObject
    {
        public MethodInfo Method = method;

        public override string Serialize()
        {
            var parameters = Method.GetParameters();

            string paramTexts = "";

            var nameList = new List<string>
            {
                "OnEvent",
                "InvokeBoolEvent",
                "InvokeEvent"
            };

            if (Method.ReflectedType == typeof(EventAPI) || Method.ReflectedType.IsSubclassOf(typeof(SubEventAPI)))
            {
                bool replaceEvent = nameList.Contains(Method.Name);

                var prefix = method.ReflectedType switch
                {
                    Type t when t == typeof(EventAPI) => "Common",
                    Type t when t == typeof(ItemEventAPI) => "Item",
                    Type t when t == typeof(TileEventAPI) => "Tile",
                    _ => ""
                };

                paramTexts = string.Join(", ", method.GetParameters().Select(p =>
                {
                    var @default = p.IsOptional ? " = " + Default2String(p.DefaultValue) : "";

                    if(replaceEvent && p.Name == "eventName")
                        return $"{p.Name}: {prefix}Event";

                    return $"{p.Name}: {Type2ClassName(p.ParameterType)}{@default}";
                }));
            }
            else
            {
                paramTexts = string.Join(", ", method.GetParameters().Select(p =>
                {
                    var @default = p.IsOptional ? " = " + Default2String(p.DefaultValue) : "";

                    return $"{p.Name}: {Type2ClassName(p.ParameterType)}{@default}";
                }));
            }
            return $"{(method.IsStatic ? "static " : "")}\"{method.Name}\"({paramTexts}): {Type2ClassName(method.ReturnType)}";
        }

        public string Default2String(object @default)
        {
            switch (@default)
            {
                case string:
                    return $"\"{@default}\"";

                case null:
                    return "null";

                default:
                    return @default.ToString();
            }
        }
    }
}
