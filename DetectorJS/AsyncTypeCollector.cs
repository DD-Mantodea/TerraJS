using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jint.Runtime;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using Terraria;

namespace TerraJS.DetectorJS
{
    public static class AsyncTypeCollector
    {
        private static readonly ConcurrentDictionary<Type, Type[]> _typeCache = new();
        private static readonly SemaphoreSlim _cacheSemaphore = new SemaphoreSlim(1, 1);

        public static async Task<HashSet<Type>> CollectAllRelatedTypesAsync(IEnumerable<Type> initialTypes)
        {
            var collectedTypes = new ConcurrentBag<Type>();
            var processedTypes = new ConcurrentDictionary<Type, bool>();
            var tasks = new List<Task>();

            foreach (var type in initialTypes)
            {
                tasks.Add(ProcessTypeAsync(type, collectedTypes, processedTypes));
            }
            await Task.WhenAll(tasks);

            var resultTypes = collectedTypes.Where(t =>
            {
                return !t.IsIllegal() && 
                t.GetCustomAttribute<HideToJSAttribute>() == null &&
                !(t.FullName?.Contains("ObjectiveCMarshal") ?? false);
            });

            return [..resultTypes];
        }

        private static async Task ProcessTypeAsync(Type type,
            ConcurrentBag<Type> collectedTypes,
            ConcurrentDictionary<Type, bool> processedTypes)
        {
            // 跳过已处理和不需处理的类型
            if (!ShouldProcessType(type) || processedTypes.ContainsKey(type))
                return;

            // 标记为已处理
            processedTypes.TryAdd(type, true);
            collectedTypes.Add(type);

            // 获取相关类型(异步缓存)
            var relatedTypes = await GetRelatedTypesAsync(type);

            // 并行处理相关类型
            var processingTasks = new List<Task>();
            foreach (var relatedType in relatedTypes)
            {
                processingTasks.Add(ProcessTypeAsync(relatedType, collectedTypes, processedTypes));
            }

            await Task.WhenAll(processingTasks);
        }

        private static async Task<Type[]> GetRelatedTypesAsync(Type type)
        {
            // 先从缓存读取
            if (_typeCache.TryGetValue(type, out var cachedTypes))
                return cachedTypes;

            // 异步获取类型信息
            var types = await Task.Run(() => UncachedGetRelatedTypes(type).Where(t =>
            {
                return !t.IsGenericParameter;
            }).ToArray());

            // 安全地更新缓存
            await _cacheSemaphore.WaitAsync();
            try
            {
                return _typeCache.GetOrAdd(type, types);
            }
            finally
            {
                _cacheSemaphore.Release();
            }
        }

        private static IEnumerable<Type> UncachedGetRelatedTypes(Type type)
        {
            // 基类和接口
            if (type.BaseType != null)
                yield return type.BaseType;

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                yield return type.GetGenericTypeDefinition();

            foreach (var interfaceType in type.GetInterfaces())
                yield return interfaceType;

            // 泛型参数
            foreach (var genericArg in type.GetGenericArguments())
                yield return genericArg;

            // 字段类型
            foreach (var field in type.GetFields())
                yield return field.FieldType;

            // 属性类型
            foreach (var property in type.GetProperties())
            {
                yield return property.PropertyType;

                var getMethod = property.GetMethod;
                if (getMethod != null)
                {
                    foreach (var param in getMethod.GetParameters())
                        yield return param.ParameterType;
                }

                var setMethod = property.SetMethod;
                if (setMethod != null)
                {
                    foreach (var param in setMethod.GetParameters())
                        yield return param.ParameterType;
                }
            }

            // 方法返回类型和参数
            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType != typeof(void))
                    yield return method.ReturnType;

                foreach (var param in method.GetParameters())
                    yield return param.ParameterType;

                foreach (var genericParam in method.GetGenericArguments())
                    yield return genericParam;
            }

            // 事件类型
            foreach (var eventInfo in type.GetEvents())
                yield return eventInfo.EventHandlerType;
        }

        private static bool ShouldProcessType(Type type)
        {
            if (type == null || type.IsPointer || type == typeof(void) || type == typeof(void*))
                return false;

            if (type.Name.Contains('&') || type.Name.Contains('*'))
                return false;

            return true;
        }
    }
}
