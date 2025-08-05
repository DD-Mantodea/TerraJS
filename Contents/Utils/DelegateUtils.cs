using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jint.Native;
using Jint;
using TerraJS.JSEngine;
using Jint.Runtime.Interop;
using Jint.Native.Function;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using Microsoft.Build.Tasks;
using System.Runtime.CompilerServices;

namespace TerraJS.Contents.Utils
{
    public class DelegateUtils
    {
        public static Type GetDelegateType(bool isFunc, int parameterCount)
        {
            if (isFunc)
            {
                return (parameterCount + 1) switch
                {
                    > 17 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    <= 0 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    <= 17 => Type.GetType($"System.Func`{parameterCount + 1}")
                };
            }
            else
            {
                return parameterCount switch
                {
                    0 => typeof(Action),
                    > 16 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    < 0 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    >= 0 => Type.GetType($"System.Action`{parameterCount}")
                };
            }
        }
    
        public static Delegate CreateHook(Delegate @delegate, Type hookType)
        {
            var method = @delegate.Method;

            var parameters = RegistryUtils.Parameters2Types(method.GetParameters());

            var hookMethod = TJSEngine.Engine.TypeConverter.Convert(@delegate, hookType, CultureInfo.InvariantCulture);

            var paramExpressions = new ParameterExpression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                paramExpressions[i] = Expression.Parameter(parameters[i], parameters[i].Name);

            return @delegate;
        }
    }
}
