using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;

namespace TerraJS.DetectorJS
{
    public static class AsyncTypeCollector
    {
        private const int BatchSize = 4096;

        private static readonly ConcurrentDictionary<Type, Type[]> _typeCache = new();

        private static int _skipped;

        public static int SkippedCount => Volatile.Read(ref _skipped);

        public static async Task<HashSet<Type>> CollectAllRelatedTypesAsync(IEnumerable<Type> initialTypes, CancellationToken token = default)
        {
            Interlocked.Exchange(ref _skipped, 0);

            var queue = new ConcurrentQueue<Type>();

            var seen = new ConcurrentDictionary<Type, byte>();

            var collected = new ConcurrentDictionary<Type, byte>();

            foreach (var type in initialTypes)
            {
                if (type is not null)
                    queue.Enqueue(type);
            }

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = token
            };

            while (!queue.IsEmpty)
            {
                token.ThrowIfCancellationRequested();

                var batch = new List<Type>();

                while (batch.Count < BatchSize && queue.TryDequeue(out var type))
                    batch.Add(type);

                await Parallel.ForEachAsync(batch, options, (type, _) =>
                {
                    if (type is null || !seen.TryAdd(type, 0))
                        return ValueTask.CompletedTask;

                    try
                    {
                        if (!ShouldProcessType(type))
                            return ValueTask.CompletedTask;

                        collected.TryAdd(type, 0);

                        foreach (var related in GetRelatedTypes(type))
                            queue.Enqueue(related);
                    }
                    catch
                    {
                        Interlocked.Increment(ref _skipped);
                    }

                    return ValueTask.CompletedTask;
                });
            }

            token.ThrowIfCancellationRequested();

            var result = new HashSet<Type>();

            foreach (var type in collected.Keys)
            {
                try
                {
                    if (IsWanted(type))
                        result.Add(type);
                }
                catch
                {
                    Interlocked.Increment(ref _skipped);
                }
            }

            return result;
        }

        private static bool IsWanted(Type type)
        {
            return !type.IsIllegal() &&
                type.GetCustomAttribute<HideToJSAttribute>() == null &&
                !(type.FullName?.Contains("ObjectiveCMarshal") ?? false);
        }

        private static Type[] GetRelatedTypes(Type type)
        {
            if (_typeCache.TryGetValue(type, out var cachedTypes))
                return cachedTypes;

            var types = UncachedGetRelatedTypes(type)
                .Where(t => t is not null && !t.IsGenericParameter)
                .Distinct()
                .ToArray();

            return _typeCache.GetOrAdd(type, types);
        }

        private static bool ShouldProcessType(Type type)
        {
            if (type is null || type == typeof(void))
                return false;

            if (type.IsGenericParameter || type.IsByRef || type.IsPointer)
                return false;

            return true;
        }

        private static IEnumerable<Type> UncachedGetRelatedTypes(Type type)
        {
            if (type.BaseType != null)
                yield return type.BaseType;

            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                yield return type.GetGenericTypeDefinition();

            foreach (var interfaceType in type.GetInterfaces())
                yield return interfaceType;

            foreach (var genericArg in type.GetGenericArguments())
                yield return genericArg;

            if (type.IsArray || type.IsByRef || type.IsPointer)
                yield return type.GetElementType();

            foreach (var field in type.GetFields())
                yield return Unwrap(field.FieldType);

            foreach (var property in type.GetProperties())
            {
                yield return Unwrap(property.PropertyType);

                var getMethod = property.GetMethod;

                if (getMethod != null)
                {
                    foreach (var param in getMethod.GetParameters())
                        yield return Unwrap(param.ParameterType);
                }

                var setMethod = property.SetMethod;

                if (setMethod != null)
                {
                    foreach (var param in setMethod.GetParameters())
                        yield return Unwrap(param.ParameterType);
                }
            }

            foreach (var constructor in type.GetConstructors())
            {
                foreach (var param in constructor.GetParameters())
                    yield return Unwrap(param.ParameterType);
            }

            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType != typeof(void))
                    yield return Unwrap(method.ReturnType);

                foreach (var param in method.GetParameters())
                    yield return Unwrap(param.ParameterType);

                foreach (var genericParam in method.GetGenericArguments())
                    yield return genericParam;
            }

            foreach (var eventInfo in type.GetEvents())
            {
                if (eventInfo.EventHandlerType != null)
                    yield return Unwrap(eventInfo.EventHandlerType);
            }
        }

        private static Type Unwrap(Type type)
        {
            if (type is null)
                return null;

            if (type.IsByRef || type.IsPointer)
                return type.GetElementType();

            return type;
        }
    }
}
