using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;

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
                else if (i.Item2 is Delegate @delegate)
                {
                    var t = @delegate.GetType();

                    var m = t.GetMethod("Invoke");

                    foreach (var p in m.GetParameters())
                        AddImport(p.ParameterType);

                    AddImport(m.ReturnType);
                }
                else if (i.Item2 is not ExpandoObject)
                    AddImport(i.Item2.GetType());

                Items.TryAdd(new(i));
            }

            foreach (var i in Imports)
                ret.AppendLine(i.Serialize());

            ret.AppendLine("declare global {");

            foreach (var i in Items)
                if(i.Serialize() != "")
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
                return $"class {Object.Item1} extends {Type2ClassName(type)} {{}}";
            else if (Object.Item2 is Delegate @delegate)
            {
                var t = @delegate.GetType();

                var m = t.GetMethod("Invoke");

                return new DetectorMethod(m).Serialize().Replace("\"Invoke\"", $"function {Object.Item1}");
            }
            else if (Object.Item2 is not ExpandoObject)
                return $"const {Object.Item1}: {Type2ClassName(Object.Item2.GetType())}";
            return "";
        }
    }
}
