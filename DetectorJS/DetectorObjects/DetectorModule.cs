using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Commands.CommandArguments;
using TerraJS.API.Events;
using TerraJS.Contents.Extensions;
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
                        if (method.GetCustomAttribute<ExtensionAttribute>() == null)
                        {
                            AddImport(method.ReturnType);

                            foreach (var p in method.GetParameters())
                                AddImport(p.ParameterType);

                            if (method.IsGenericMethod)
                                AddImport(typeof(Type));
                        }
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

            Classes.TryAdd(new(type, this));
        }

        public void AddImport(Type type)
        {
            if (type.IsArray || type.IsIllegal())
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

        public void AddExtensions()
        {
            foreach (var clazz in Classes)
                clazz.AddExtensionMethods();
        }

        public override string Serialize()
        {
            var ret = new StringBuilder();

            ret.AppendLine($"declare module \"{ModuleName}\" {{");

            foreach (var import in Imports)
                ret.AppendLine(import.Serialize());

            foreach (var clazz in Classes)
                ret.AppendLine(clazz.Serialize());

            ret.AppendLine("}");

            return ret.ToString();
        }
    }
}
