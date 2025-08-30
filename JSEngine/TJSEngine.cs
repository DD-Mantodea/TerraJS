using System;
using System.Reflection;
using Jint;
using Terraria.ModLoader.Core;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TerraJS.API;
using TerraJS.Contents.Utils;
using Terraria.Localization;
using TerraJS.Contents.Attributes;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Jint.Runtime.Interop;
using System.Collections;
using TerraJS.JSEngine.Plugins;

namespace TerraJS.JSEngine
{
    [HideToJS]
    public class TJSEngine
    {
        public static Engine Engine;

        public static GlobalAPI GlobalAPI = new();

        public static List<TJSPlugin> Plugins = new();

        public static void Load()
        {
            Assembly[] assemblies = [
                ..AssemblyManager.GetModAssemblies("TerraJS"),
                typeof(ModLoader).Assembly,
                typeof(Vector2).Assembly,
                ..ModLoader.Mods.Select(m => m.Code),
                GlobalAPI._ab
            ];

            var exts = AssemblyUtils.GetExtensionTypes([.. assemblies.Distinct()]);

            Engine = new Engine(option => {
                option.AllowClr(assemblies);
                option.AddExtensionMethods([..exts]);
                option.Interop.AllowSystemReflection = true;
                option.Interop.AllowGetType = true;
                option.AllowOperatorOverloading();
                option.CatchClrExceptions(exception =>
                {
                    Console.WriteLine($"[Jint] CLR Exception: {exception.Message}");
                    return true;
                });
            });

            BindingUtils.Clear();

            BindingUtils.BindInstance("TJS", GlobalAPI);

            BindingUtils.BindStaticOrEnumOrConst("Cultures", typeof(GameCulture.CultureName));

            BindInnerMethods();

            GlobalAPI.Unload();

            LoadScripts();
        }

        public static void BindInnerMethods()
        {
            Engine.SetValue("require", (string path) => Engine.Evaluate($"importNamespace(\"{path}\")"));

            BindingUtils.BindInnerMethod("nettypeof", (TypeReference t) => t.ReferenceType);
        }

        public static void LoadScripts()
        {
            FileUtils.CreateDirectoryIfNotExist(Pathes.TerraJSPath);

            FileUtils.CreateDirectoryIfNotExist(Path.Combine(Pathes.TerraJSPath, "Scripts"));

            FileUtils.CreateDirectoryIfNotExist(Path.Combine(Pathes.TerraJSPath, "Textures"));

            var files = Directory.GetFiles(Path.Combine(Pathes.TerraJSPath, "Scripts"), "*.js", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string script = File.ReadAllText(file);

                Engine.Execute($"(function() {{ {script} }})()", file);
            }
        }

        public static void LoadPlugins()
        {
            var plugins = AssemblyUtils.GetPluginTypes([.. ModLoader.Mods.Select(m => m.Code)]);

            foreach (var plugin in plugins)
            {
                var instance = Activator.CreateInstance(plugin) as TJSPlugin;

                Plugins.Add(instance);

                instance.Load();
            }
        }

        public static void Unload()
        {
            GlobalAPI.Unload();

            Engine.Dispose();

            BindingUtils.Values.Clear();

            Engine = null;

            GlobalAPI = null;

            TerraJS.Instance.Logger.Info("[Jint] JSEngine unloaded successfully.");
        }
    }
}
