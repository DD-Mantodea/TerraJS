using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Interop;
using MonoMod.Cil;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine;
using Terraria.ModLoader;

namespace TerraJS.API.Reflections
{
    public class HookAPI : BaseAPI
    {
        [HideToJS]
        public static Dictionary<string, List<MulticastDelegate>> JsHooks = [];

        public void AddHook(MethodInfo methodInfo, Delegate @delegate)
        {
            if (!TerraJS.IsLoading)
                return;

            var paramTypes = RegistryUtils.Parameters2Types(methodInfo.GetParameters());

            Type[] self = methodInfo.IsStatic ? [] : [methodInfo.ReflectedType ];

            var origType = DelegateUtils.GetDelegateType(methodInfo.ReturnType != typeof(void), paramTypes.Length + self.Length).MakeGenericType([..self, ..paramTypes]);

            Type[] hookParamTypes = [origType, .. self, .. paramTypes];

            var hookType = DelegateUtils.GetDelegateType(methodInfo.ReturnType != typeof(void), paramTypes.Length + self.Length + 1).MakeGenericType(hookParamTypes);

            var hookMethod = TJSEngine.Engine.TypeConverter.Convert(@delegate, hookType, CultureInfo.InvariantCulture) as MulticastDelegate;

            var methodKey = $"{methodInfo.ReflectedType.FullName}.{methodInfo.Name}";

            JsHooks.TryAdd(methodKey, []);

            JsHooks[methodKey].Add(hookMethod);

            var hookIndex = JsHooks[methodKey].Count - 1;

            var method = new DynamicMethod($"{methodInfo.Name}_hook_{hookIndex}", methodInfo.ReturnType, hookParamTypes, GetType());

            var il = method.GetILGenerator();

            il.DeclareLocal(typeof(object[]));

            il.Emit(OpCodes.Ldc_I4, hookParamTypes.Length);

            il.Emit(OpCodes.Newarr, typeof(object));

            for (int i = 0; i < hookParamTypes.Length; i++)
            {
                il.Emit(OpCodes.Dup);

                il.Emit(OpCodes.Ldc_I4, i);

                il.Emit(OpCodes.Ldarg, i);

                if (hookParamTypes[i].IsValueType)
                    il.Emit(OpCodes.Box, hookParamTypes[i]);

                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Stloc_0);

            il.Emit(OpCodes.Ldsfld, GetType().GetField("JsHooks"));

            il.Emit(OpCodes.Ldstr, methodKey);

            il.Emit(OpCodes.Callvirt, typeof(Dictionary<string, List<MulticastDelegate>>).GetMethod("get_Item"));

            il.Emit(OpCodes.Ldc_I4, hookIndex);

            il.Emit(OpCodes.Callvirt, typeof(List<MulticastDelegate>).GetMethod("get_Item"));

            il.Emit(OpCodes.Ldloc_0);

            il.Emit(OpCodes.Callvirt, typeof(MulticastDelegate).GetMethod("DynamicInvoke"));

            if (methodInfo.ReturnType != typeof(void))
            {
                if (methodInfo.ReturnType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                else
                    il.Emit(OpCodes.Castclass, methodInfo.ReturnType);
            }
            else
                il.Emit(OpCodes.Pop);

            il.Emit(OpCodes.Ret);

            var a = il.ToString();

            MonoModHooks.Add(methodInfo, method.CreateDelegate(hookType));

            return;
        }

        public JsValue CallOrig(Func<JsValue, JsValue[], JsValue> method, JsValue[] parameters)
        {
            return method?.Invoke(null, parameters);
        }

        public void AddILHook(MethodInfo methodInfo, Action<ILContext> @delegate)
        {
            MonoModHooks.Modify(methodInfo, (il) => @delegate(il));
        }

        internal override void Unload() { }
    }
}
