using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.DetectorJS.DetectorObjects;
using TerraJS.JSEngine.API;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace TerraJS.DetectorJS
{
    public class Detector
    {
        internal static ConcurrentDictionary<string, DetectorModule> Modules = [];

        internal static ConcurrentDictionary<Type, List<MethodInfo>> GenericExtensionMethods = [];

        internal static ConcurrentDictionary<Type, List<MethodInfo>> ExtensionMethods = [];

        private static readonly ConcurrentQueue<Action> MainThreadActions = new();

        private static int _running;

        private static CancellationTokenSource _cancellation;

        public static void Detect()
        {
            if (Interlocked.CompareExchange(ref _running, 1, 0) != 0)
            {
                Main.NewText(Language.ActiveCulture.LegacyId == 7 ? "[Detector] 正在检测中" : "[Detector] Detection is already running", 255, 80, 80);

                return;
            }

            var snapshot = new DetectorSnapshot
            {
                Assemblies = [
                    ..AssemblyManager.GetModAssemblies("TerraJS"),
                    ..ModLoader.Mods.Select(m => m.Code),
                    GlobalAPI._ab
                ],
                Bindings = [..BindingUtils.Values.Select(i => i.Item2)],
            };

            _cancellation?.Dispose();

            _cancellation = new CancellationTokenSource();

            var token = _cancellation.Token;

            new Thread(() => Run(snapshot, token)) {
                IsBackground = true,
                Name = "TerraJS Detector"
            }.Start();
        }

        public static void Cancel()
        {
            try
            {
                _cancellation?.Cancel();
            }
            catch (ObjectDisposedException)
            {

            }
        }

        public static void FlushMainThreadActions()
        {
            while (MainThreadActions.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    TerraJS.Instance?.Logger.Error($"[Detector] {e}");
                }
            }
        }

        private static void Run(DetectorSnapshot snapshot, CancellationToken token)
        {
            try
            {
                Modules = [];

                ExtensionMethods = [];

                GenericExtensionMethods = [];

                var allTypes = new List<Type>();

                allTypes.AddRange(typeof(Detector).Assembly.GetTypes());

                allTypes.AddRange(typeof(ModLoader).Assembly.GetTypes());

                allTypes.AddRange(typeof(Vector2).Assembly.GetTypes());

                foreach (var assembly in snapshot.Assemblies)
                    allTypes.AddRange(SafeGetTypes(assembly));

                foreach (var binding in snapshot.Bindings)
                {
                    if (binding is Type type)
                        allTypes.TryAdd(type);
                    else if (binding is not ExpandoObject)
                        allTypes.TryAdd(binding.GetType());
                }

                allTypes = [..allTypes.Where(t => !t.IsIllegal())];

                QueueMainThread(() => Main.NewText(Language.ActiveCulture.LegacyId == 7 ? "[Detector] 正在收集所需的类型" : "[Detector] Collecting needed types"));

                var stopwatch = Stopwatch.StartNew();

                var types = AsyncTypeCollector.CollectAllRelatedTypesAsync(allTypes, token).GetAwaiter().GetResult();

                var packagePath = Path.Combine(Pathes.TerraJSPath, "Packages");

                if (Directory.Exists(packagePath))
                    Directory.Delete(packagePath, true);

                Directory.CreateDirectory(packagePath);

                var namespaceGroups = types
                    .Where(t => t.Name != "")
                    .GroupBy(t => t.Namespace ?? (t.IsGlobalNamespace() ? "GlobalNamespace" : string.Empty))
                    .Where(g => g.Key != string.Empty)
                    .OrderBy(g => g.Key)
                    .ToList();

                Parallel.ForEach(namespaceGroups, new ParallelOptions {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                },
                group => {
                    var module = new DetectorModule(group.Key);

                    Modules.TryAdd(group.Key, module);

                    foreach (var type in group.OrderBy(t => t.FullName, StringComparer.Ordinal))
                        module.AddType(type);
                });

                Parallel.ForEach(Modules, new ParallelOptions {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                },
                pair => {
                    var filePath = Path.Combine(packagePath, $"{pair.Key}.d.ts");

                    pair.Value.AddExtensions();

                    File.WriteAllText(filePath, pair.Value.Serialize());
                });

                File.WriteAllText(Path.Combine(packagePath, "global.d.ts"), new DetectorGlobal().Serialize());

                WriteJsConfig(packagePath);

                stopwatch.Stop();

                var count = types.Count;

                var skipped = AsyncTypeCollector.SkippedCount;

                var elapsed = stopwatch.ElapsedMilliseconds;

                Modules = [];

                ExtensionMethods = [];

                GenericExtensionMethods = [];

                QueueMainThread(() => Main.NewText(Language.ActiveCulture.LegacyId == 7
                    ? $"[Detector] 写入完成, 共 {count} 个类型, 跳过 {skipped} 个, 共耗时 {elapsed}ms"
                    : $"[Detector] Write complete, {count} types in total, {skipped} skipped, consuming {elapsed}ms"));

                QueueMainThread(TerraJS.Reload);
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                var message = $"[Detector] {e}";

                QueueMainThread(() => Main.NewText(message, 255, 80, 80));
            }
            finally
            {
                Volatile.Write(ref _running, 0);
            }
        }

        private static void WriteJsConfig(string packagePath)
        {
            var options = new JObject
            {
                ["module"] = "commonjs",
                ["moduleResolution"] = "classic",
                ["isolatedModules"] = true,
                ["composite"] = true,
                ["incremental"] = true,
                ["allowJs"] = true,
                ["checkJs"] = false,
                ["target"] = "ES2023",
                ["rootDir"] = "./Scripts",
                ["baseUrl"] = "./Packages",
                ["skipLibCheck"] = true,
                ["skipDefaultLibCheck"] = true,
                ["lib"] = new JArray
                {
                    "ES6",
                    "ES2023"
                },
                ["typeRoots"] = new JArray
                {
                    "./Packages"
                }
            };

            var config = new JObject
            {
                ["include"] = new JArray
                {
                    "./**/*.ts",
                    "./**/*.js"
                },
                ["compilerOptions"] = options
            };

            File.WriteAllText(Path.Combine(Pathes.TerraJSPath, "jsconfig.json"), config.ToString());
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t is not null);
            }
            catch
            {
                return [];
            }
        }

        private static void QueueMainThread(Action action) => MainThreadActions.Enqueue(action);

        private sealed class DetectorSnapshot
        {
            public Assembly[] Assemblies;

            public object[] Bindings;
        }
    }
}
