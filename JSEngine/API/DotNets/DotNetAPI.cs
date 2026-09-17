using Acornima.Ast;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API.Items;
using Terraria;

namespace TerraJS.JSEngine.API.DotNets
{
    public class DotNetAPI : BaseAPI
    {
        /*
        public EnumRegistry CreateEnumRegistry(string name, string @namespace = "")
        {
            if (string.IsNullOrWhiteSpace(name) || @namespace.IsNullOrWhiteSpaceNotEmpty())
            {
                return Enums.EnumRegistry.Empty;
            }

            var enumName = $"TJSContents.Enums.{(@namespace == "" ? "" : @namespace + ".")}{name}";

            EnumBuilder builder = GlobalAPI._mb.DefineEnum(enumName, TypeAttributes.Public, typeof(int));

            var registry = new EnumRegistry(builder);

            return registry;
        }
        */

        public Type CreateType(ScriptFunction jsClassDef)
        {
            var name = jsClassDef.NameDescriptor.Value.AsString();

            var baseType = (jsClassDef.Prototype as TypeReference).ReferenceType;

            var builder = GlobalAPI._mb.DefineType($"TJSContents.JSTypes.{name}", TypeAttributes.Public, baseType);

            var fieldList = new List<(FieldBuilder builder, object value)>();

            var staticList = new List<(string name, object value)>();

            foreach (var staticField in jsClassDef.StaticFields)
            {
                var fieldName = staticField.Key.AsString();

                var value = staticField.Value.ToObject();

                builder.DefineField(fieldName, value.GetType(), FieldAttributes.Public | FieldAttributes.Static);

                staticList.Add((fieldName, value));
            }

            foreach (var field in jsClassDef.Fields)
            {
                var fieldName = field.Name.AsString();

                var value = field.Initializer.Call(null, []).ToObject();

                switch (value)
                {
                    case Func<JsValue, JsValue[], JsValue> @delegate:
                    {
                        var method = typeof(TJSItem).GetMethod(fieldName);

                        if (method == null)
                        {
                            var fieldBuilder = builder.DefineField(fieldName, typeof(Func<JsValue, JsValue[], JsValue>), FieldAttributes.Public);

                            fieldList.Add((fieldBuilder, value));
                        }
                        else
                        {
                            RegistryUtils.OverrideJS(builder, fieldName);

                            staticList.Add(($"{fieldName}_Delegate", @delegate));
                        }

                        break;
                    }

                    case null:
                    {
                        builder.DefineField(fieldName, value.GetType(), FieldAttributes.Public);

                        break;
                    }

                    default:
                    {
                        var fieldBuilder = builder.DefineField(fieldName, value.GetType(), FieldAttributes.Public);

                        fieldList.Add((fieldBuilder, value));

                        break;
                    }
                }
            }

            RegistryUtils.DefineConstructor(builder, fieldList);

            var type = builder.CreateType();

            foreach (var @static in staticList)
                type.GetField(@static.name).SetValue(null, @static.value);

            foreach (var field in fieldList)
                type.GetField($"_{field.builder.Name}_init", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, field.value);

            return type;
        }

        internal override void Unload()
        {

        }
    }
}
