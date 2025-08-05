using Jint;
using Jint.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Items;
using Terraria.ModLoader;
using Terraria;
using TerraJS.JSEngine;
using TerraJS.API;

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
                method.Attributes,
                method.CallingConvention,
                method.ReturnType,
                Parameters2Types(method.GetParameters())
            );
        }

        public static MethodBuilder CreateMethodBuilder(TypeBuilder builder, MethodInfo method)
        {
            return builder.DefineMethod(
                method.Name,
                method.Attributes,
                method.CallingConvention,
                method.ReturnType,
                Parameters2Types(method.GetParameters())
            );
        }

        public static void Override<T>(Registry<T> registry, string methodName, MulticastDelegate @delegate) where T : ModType
        {
            var field = registry._builder.DefineField($"{methodName}Delegate", @delegate.GetType(), FieldAttributes.Public | FieldAttributes.Static);

            registry.AfterRegister += (Type type) =>
            {
                type.GetField($"{methodName}Delegate").SetValue(null, @delegate);
            };

            var baseMethod = registry._builder.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

            var parameters = baseMethod.GetParameters();

            var method = CreateMethodBuilder(registry._builder, baseMethod);

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldsfld, field);

            for (int i = 0; i < parameters.Length; i++)
                il.Emit(OpCodes.Ldarg, i + 1);

            il.Emit(OpCodes.Callvirt, @delegate.GetType().GetMethod("Invoke"));

            il.Emit(OpCodes.Ret);

            //registry._builder.DefineMethodOverride(method, baseMethod);
        }
    }
}
