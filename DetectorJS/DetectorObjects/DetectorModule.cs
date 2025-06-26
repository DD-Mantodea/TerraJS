using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
using TerraJS.Extensions;
using Terraria;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorModule(string moduleName) : DetectorObject
    {
        public string ModuleName = moduleName;

        public List<DetectorImport> Imports = [];

        public List<DetectorClass> Classes = [];

        public void AddType(Type type)
        {
            if (type.IsArray || (type.IsGenericType && !type.IsGenericTypeDefinition) || type.IsGenericTypeParameter)
                return;

            AddImport(type);

            if (type.BaseType != null)
                AddImport(type.BaseType);

            AddClass(type);

            foreach (var i in type.GetMembers())
            {
                switch (i)
                {
                    case PropertyInfo property:
                        AddImport(property.PropertyType);
                        break;

                    case FieldInfo field:
                        AddImport(field.FieldType);
                        break;

                    case MethodInfo method:
                        AddImport(method.ReturnType);
                        foreach (var p in method.GetParameters())
                            AddImport(p.ParameterType);
                        break;

                    default:
                        break;
                }
            }
        }

        public void AddClass(Type type)
        {
            if (type.IsArray || (type.IsGenericType && !type.IsGenericTypeDefinition) || type.IsGenericTypeParameter)
                return;

            Classes.TryAdd(new(type));
        }

        public void AddImport(Type type)
        {
            if (type.IsPointer || type.IsByRef || type.IsGenericParameter)
                return;

            var import = new DetectorImport(Type2ImportName(type), type.Namespace);

            if (type.IsGenericType)
            {
                if (!type.IsGenericTypeDefinition)
                {
                    import = new(Type2ImportName(type.GetGenericTypeDefinition()), type.GetGenericTypeDefinition().Namespace);

                    foreach (var arg in type.GetGenericArguments())
                        AddImport(arg);
                }
            }

            if (type.IsArray)
            {
                if (!type.GetElementType().IsGenericParameter)
                    import = new(Type2ImportName(type.GetElementType()), type.GetElementType().Namespace);
                else return;
            }

            if (type.IsGlobalNamespace())
                import = new(Type2ImportName(type), "GlobalNamespace");

            if (import.ModuleName != ModuleName)
                Imports.TryAdd(import);
        }

        public override string Serialize()
        {
            var ret = new StringBuilder();

            ret.AppendLine($"declare module \"{ModuleName}\" {{");

            foreach (var import in Imports)
                ret.AppendLine(import.Serialize());

            if (ModuleName == "TerraJS.API.Events")
            {
                ret.AppendLine("type CommonEvent = " + string.Join(" | ", TerraJS.GlobalAPI.Event.Events.Keys.Select(s => $"\"{s}\"")));
                ret.AppendLine("type ItemEvent = " + string.Join(" | ", TerraJS.GlobalAPI.Event.Item.Events.Keys.Select(s => $"\"{s}\"")));
                ret.AppendLine("type TileEvent = " + string.Join(" | ", TerraJS.GlobalAPI.Event.Tile.Events.Keys.Select(s => $"\"{s}\"")));
            }

            foreach (var clazz in Classes)
                ret.AppendLine(clazz.Serialize());

            ret.AppendLine("}");

            return ret.ToString();
        }
    }
}
