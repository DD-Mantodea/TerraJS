﻿using System;
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
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using TerraJS.DetectorJS.DetectorObjects;
using TerraJS.JSEngine;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.DetectorJS
{
    public class Detector
    {

        internal static ConcurrentDictionary<string, DetectorModule> Modules = [];

        internal static ConcurrentDictionary<Type, List<MethodInfo>> ExtensionMethods = [];

        public static void Detect()
        {
            List<Type> allTypes = [
                ..typeof(Detector).Assembly.GetTypes(), 
                ..typeof(Main).Assembly.GetTypes(), 
                ..typeof(Vector2).Assembly.GetTypes(),
                ..TJSEngine.CustomTypes
            ];

            BindingUtils.Values.ForEach(i =>
            {
                if (i.Item2 is Type type)
                    allTypes.TryAdd(type);
                else if (i.Item2 is not ExpandoObject)
                    allTypes.TryAdd(i.Item2.GetType());
                else allTypes.TryAddRange([.. ((IDictionary<string, object>)i.Item2).Select(o => o.GetType())]);
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

                    var packagePath = Path.Combine(TerraJS.ModPath, "Packages");

                    if (!Directory.Exists(packagePath))
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

                        File.WriteAllText(filePath, pair.Value.Serialize());
                    });

                    File.WriteAllText(Path.Combine(TerraJS.ModPath, "Packages", "global.d.ts"), new DetectorGlobal().Serialize());

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

                    File.WriteAllText(Path.Combine(TerraJS.ModPath, "jsconfig.json"), config.ToString());

                    #endregion

                    Modules = [];

                    ExtensionMethods = [];
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
