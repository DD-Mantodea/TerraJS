using Jint;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Utils;
using TerraJS.JSEngine.API;
using TerraJS.JSEngine.Plugins;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

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
            GlobalAPI ??= new();

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

            BindingUtils.Flush();

            BindingUtils.BindInstance("TJS", GlobalAPI);

            BindingUtils.BindStaticOrEnumOrConst("Cultures", typeof(GameCulture.CultureName));

            BindInnerMethods();

            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.AddBindings();
                }
                catch (Exception e)
                {
                    TerraJS.Instance?.Logger.Error($"[Plugin] {plugin.GetType().FullName} failed to bind: {e}");
                }
            }

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
                try
                {
                    if (Activator.CreateInstance(plugin) is not TJSPlugin instance)
                    {
                        TerraJS.Instance?.Logger.Warn($"[Plugin] {plugin.FullName} could not be instantiated, skipped.");

                        continue;
                    }

                    var owner = ModLoader.Mods.FirstOrDefault(mod => mod.Code == plugin.Assembly);

                    if (owner is null)
                    {
                        TerraJS.Instance?.Logger.Warn($"[Plugin] {plugin.FullName} has no owning mod, skipped.");

                        continue;
                    }

                    owner.AddContent(instance);

                    Plugins.Add(instance);

                    if (instance.Mod is null)
                        TerraJS.Instance?.Logger.Warn($"[Plugin] {plugin.FullName}: AddContent did not assign a Mod, its tML lifecycle will not run.");
                    else
                        TerraJS.Instance?.Logger.Info($"[Plugin] {plugin.FullName} loaded by mod {owner.Name}.");
                }
                catch (Exception e)
                {
                    TerraJS.Instance?.Logger.Error($"[Plugin] {plugin.FullName} failed to load: {e}");
                }
            }
        }

        public static void Unload()
        {
            GlobalAPI?.Unload();

            Engine?.Dispose();

            BindingUtils.Values?.Clear();

            Plugins?.Clear();

            Engine = null;

            GlobalAPI = null;

            TerraJS.Instance?.Logger.Info("[Jint] JSEngine unloaded successfully.");
        }
    }
}
