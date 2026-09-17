using Jint;
using Jint.Native;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TerraJS.JSEngine;
using TerraJS.JSEngine.API;
using Terraria.IO;
using Terraria.ModLoader;

namespace TerraJS.Contents.Utils
{
    public class RegistryUtils
    {
        public static Type[] Parameters2Types(ParameterInfo[] paras)
        {
            Type[] types = new Type[paras.Length];
            var list = paras.ToList();
            foreach (var p in list)
            {
                types[list.IndexOf(p)] = p.ParameterType;
            }
            return types;
        }

        public static MethodBuilder CreateMethodBuilder(TypeBuilder builder, Type type, string methodName)
        {
            var method = type.GetMethod(methodName);

            return builder.DefineMethod(
                methodName,
                method.Attributes & ~MethodAttributes.NewSlot | MethodAttributes.ReuseSlot,
                method.CallingConvention,
                method.ReturnType,
                Parameters2Types(method.GetParameters())
            );
        }

        public static MethodBuilder CreateMethodBuilder(TypeBuilder builder, MethodInfo method)
        {
            return builder.DefineMethod(
                method.Name,
                (method.Attributes & ~MethodAttributes.NewSlot) | MethodAttributes.ReuseSlot,
                method.CallingConvention,
                method.ReturnType,
                Parameters2Types(method.GetParameters())
            );
        }

        public static void Override<T, K>(ModTypeRegistry<T, K> registry, string methodName, MulticastDelegate @delegate) where T : ModType where K : ModTypeRegistry<T, K>
        {
            var field = registry._builder.DefineField($"{methodName}_Delegate", @delegate.GetType(), FieldAttributes.Public | FieldAttributes.Static);

            registry.AfterRegister += type =>
            {
                type.GetField($"{methodName}_Delegate").SetValue(null, @delegate);
            };

            var baseMethod = registry._builder.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

            var parameters = baseMethod.GetParameters();

            var method = CreateMethodBuilder(registry._builder, baseMethod);

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldsfld, field);

            for (int i = 0; i < parameters.Length + 1; i++)
                il.Emit(OpCodes.Ldarg, i);

            il.Emit(OpCodes.Callvirt, @delegate.GetType().GetMethod("Invoke"));

            il.Emit(OpCodes.Ret);
        }

        public static FieldBuilder OverrideJS<T, K>(ModTypeRegistry<T, K> registry, string methodName, Func<JsValue, JsValue[], JsValue> @delegate) where T : ModType where K : ModTypeRegistry<T, K>
        {
            registry.AfterRegister += type =>
            {
                type.GetField($"{methodName}_Delegate").SetValue(null, @delegate);
            };

            return OverrideJS(registry._builder, methodName);
        }

        public static FieldBuilder OverrideJS(TypeBuilder builder, string methodName)
        {
            var field = builder.DefineField($"{methodName}_Delegate", typeof(Func<JsValue, JsValue[], JsValue>), FieldAttributes.Public | FieldAttributes.Static);

            var baseMethod = builder.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

            var parameters = baseMethod.GetParameters();

            var method = CreateMethodBuilder(builder, baseMethod);

            var engine = typeof(TJSEngine).GetField(nameof(TJSEngine.Engine));

            var fromObj = typeof(JsValue).GetMethod(nameof(JsValue.FromObject));

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldsfld, field);

            il.Emit(OpCodes.Ldsfld, engine);

            il.Emit(OpCodes.Ldarg, 0);

            il.Emit(OpCodes.Call, fromObj);

            if (parameters.Length == 0)
            {
                var empty = typeof(Array).GetMethod(nameof(Array.Empty)).MakeGenericMethod(typeof(JsValue));

                il.Emit(OpCodes.Call, empty);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, parameters.Length);

                il.Emit(OpCodes.Newarr, typeof(JsValue));

                il.Emit(OpCodes.Dup);
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldc_I4, i);

                il.Emit(OpCodes.Ldsfld, engine);

                il.Emit(OpCodes.Ldarg, i + 1);

                il.Emit(OpCodes.Call, fromObj);

                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Callvirt, typeof(Func<JsValue, JsValue[], JsValue>).GetMethod("Invoke"));

            if (baseMethod.ReturnType == typeof(void))
                il.Emit(OpCodes.Pop);
            else
            {
                var @as = typeof(JsValue).GetMethod("As").MakeGenericMethod(baseMethod.ReturnType);

                il.Emit(OpCodes.Callvirt, @as);
            }

            il.Emit(OpCodes.Ret);

            return field;
        }
    
        public static void DefineConstructor(TypeBuilder builder, List<(FieldBuilder builder, object value)> fields, params Type[] types)
        {
            var ctor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, types);

            var il = ctor.GetILGenerator();

            foreach (var field in fields)
            {
                var staticField = builder.DefineField($"_{field.builder.Name}_init", field.value.GetType(), FieldAttributes.Private | FieldAttributes.Static);

                il.Emit(OpCodes.Ldarg_0);

                il.Emit(OpCodes.Ldsfld, staticField);

                il.Emit(OpCodes.Stfld, field.builder);
            }

            il.Emit(OpCodes.Ret);
        }
    }
}
