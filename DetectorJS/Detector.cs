using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TerraJS.API;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.DetectorJS.DetectorObjects;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace TerraJS.DetectorJS
{
    public class Detector
    {

        internal static ConcurrentDictionary<string, DetectorModule> Modules = [];

        internal static ConcurrentDictionary<Type, List<MethodInfo>> ExtensionMethods = [];

        public static void Detect()
        {
            List<Assembly> assemblies = [
                ..AssemblyManager.GetModAssemblies("TerraJS"),
                ..ModLoader.Mods.Select(m => m.Code),
                GlobalAPI._ab
            ];

            List<Type> allTypes = [
                ..typeof(Detector).Assembly.GetTypes(), 
                ..typeof(ModLoader).Assembly.GetTypes(), 
                ..typeof(Vector2).Assembly.GetTypes()
            ];

            assemblies.ForEach(a => allTypes.AddRange(a.GetTypes()));

            BindingUtils.Values.ForEach(i =>
            {
                if (i.Item2 is Type type)
                    allTypes.TryAdd(type);
                else if (i.Item2 is not ExpandoObject)
                    allTypes.TryAdd(i.Item2.GetType());
            });

            allTypes = [..allTypes.Where(t => !t.IsIllegal())];

            var task = AsyncTypeCollector.CollectAllRelatedTypesAsync(allTypes);

            if (Language.ActiveCulture.LegacyId == 7)
                Main.NewText("[Detector] 正在收集所需的类型");
            else Main.NewText("[Detector] Collecting needed types");

            var stopwatch = Stopwatch.StartNew();

            task.ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    stopwatch.Stop();

                    var types = task.Result;

                    if (Language.ActiveCulture.LegacyId == 7)
                        Main.NewText($"[Detector] 收集完成, 共 {types.Count} 个类型, 共耗时 {stopwatch.ElapsedMilliseconds}ms");
                    else Main.NewText($"[Detector] Collect complete, {types.Count} types in total, consuming {stopwatch.ElapsedMilliseconds}ms");

                    var packagePath = Path.Combine(Pathes.TerraJSPath, "Packages");

                    if(Directory.Exists(packagePath))
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

                        foreach (var type in group)
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

                    File.WriteAllText(Path.Combine(Pathes.TerraJSPath, "Packages", "global.d.ts"), new DetectorGlobal().Serialize());

                    #region jsconfig
                    var config = new JObject
                    {
                        ["include"] = new JArray
                        {
                            "./**/*.ts",
                            "./**/*.js"
                        }
                    };

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

                    config["compilerOptions"] = options;

                    File.WriteAllText(Path.Combine(Pathes.TerraJSPath, "jsconfig.json"), config.ToString());

                    #endregion

                    Modules = [];

                    ExtensionMethods = [];

                    TerraJS.Reload();
                }
            });
        }

        public static string GetMethodComment(MethodInfo method)
        {
            if (method.GetCustomAttribute<CommentAttribute>() is { } attribute)
                return attribute.Comment;

            return "";
        }
    }
}
