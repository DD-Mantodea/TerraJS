using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using TerraJS.API;
using TerraJS.Contents.Extensions;

namespace TerraJS.JSEngine.API.DotNets.Extensions
{
    public class ExtensionRegistry : IRegistry<object>
    {
        internal TypeBuilder _builder;

        internal static Dictionary<string, Type> _tjsExtensions = [];

        public bool IsEmpty;

        public Action<Type> AfterRegister;

        public ExtensionRegistry(string name, string @namespace = "")
        {
            if (string.IsNullOrWhiteSpace(name) || @namespace.IsNullOrWhiteSpaceNotEmpty())
            {
                IsEmpty = true;

                return;
            }

            var extName = $"TJSContents.Extensions.{(@namespace == "" ? "" : @namespace + ".")}{name}";

            _builder = GlobalAPI._mb.DefineType(extName, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Class);

            _builder.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructor(Type.EmptyTypes), []));
        }

        public ExtensionRegistry CreateExtensionMethod(string methodName, Type extType, Type retType, Type[] parameterTypes, MulticastDelegate @delegate, ExtParameterInfo[] extParameterInfos = null)
        {
            if (IsEmpty)
                return this;

            var field = _builder.DefineField($"{methodName}Delegate", @delegate.GetType(), FieldAttributes.Public | FieldAttributes.Static);

            AfterRegister += (Type type) =>
            {
                type.GetField($"{methodName}Delegate").SetValue(null, @delegate);
            };

            var method = _builder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, retType, [extType, .. parameterTypes]);

            method.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructor(Type.EmptyTypes), []));

            var thisParam = method.DefineParameter(1, ParameterAttributes.None, "instance");

            thisParam.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructor(Type.EmptyTypes), []));

            if (extParameterInfos != null && extParameterInfos.Length == parameterTypes.Length)
            {
                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    var info = extParameterInfos[i];

                    var param = method.DefineParameter(i + 2, info.Attributes, info.Name);

                    if (info.defaultValue != null)
                        param.SetConstant(info.defaultValue);
                }
            }

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldsfld, field);

            for (int i = 0; i < parameterTypes.Length; i++)
                il.Emit(OpCodes.Ldarg, i + 1);

            il.Emit(OpCodes.Callvirt, @delegate.GetType().GetMethod("Invoke"));

            il.Emit(OpCodes.Ret);

            return this;
        }

        public void Register()
        {
            if (IsEmpty)
                return;

            var extType = _builder.CreateType();

            AfterRegister?.Invoke(extType);

            _tjsExtensions.Add(_builder.Name, extType);
        }
    }

    public record ExtParameterInfo(string Name, ParameterAttributes Attributes = ParameterAttributes.None, object defaultValue = null);
}
