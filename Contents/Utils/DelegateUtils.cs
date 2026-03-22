using System;
using System.Globalization;
using System.Linq.Expressions;
using TerraJS.JSEngine;

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
