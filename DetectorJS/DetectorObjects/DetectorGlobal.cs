using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using TerraJS.Extensions;
using TerraJS.Utils;
using Terraria;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorGlobal : DetectorObject
    {
        public List<DetectorImport> Imports = [];

        public List<DetectorGlobalItem> Items = [];

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

            Imports.TryAdd(import);
        }

        public override string Serialize()
        {
            var ret = new StringBuilder();

            foreach (var i in BindingUtils.Values)
            {
                if (i.Item2 is Type type)
                    AddImport(type);
                else if (i.Item2 is not ExpandoObject)
                    AddImport(i.Item2.GetType());

                Items.TryAdd(new(i));
            }

            foreach (var i in Imports)
                ret.AppendLine(i.Serialize());

            ret.AppendLine("declare global {");

            foreach (var i in Items)
                ret.AppendLine(i.Serialize());

            ret.AppendLine("}");

            return ret.ToString();
        }
    }

    public class DetectorGlobalItem((string, object) obj) : DetectorObject
    {
        public (string, object) Object = obj;

        public override string Serialize()
        {
            if (Object.Item2 is Type type)
                return $"type {Object.Item1} = {Type2ClassName(type)}";
            else if (Object.Item2 is not ExpandoObject)
                return $"const {Object.Item1}: {Type2ClassName(Object.Item2.GetType())}";

            return "";
        }
    }
}
