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
using Jint.Native;
using System.IO;
using TerraJS.JSEngine.Plugins;
using System.Collections.Generic;
using System.Linq;
using Jint.Runtime.Interop;
using MonoMod.RuntimeDetour;

namespace TerraJS.JSEngine
{
    [HideToJS]
    public class TJSEngine
    {
        public static Engine Engine;

        public static GlobalAPI GlobalAPI = new();

        internal static List<Type> CustomTypes = [];

        public static void Load()
        {
            var plugins = (Dictionary<string, TJSPlugin>)typeof(ModTypeLookup<>).MakeGenericType([typeof(TJSPlugin)]).GetField("dict", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

            var pluginAssemblies = plugins.Values.Select(p => p.GetType().Assembly).Distinct();

            Assembly[] assemblies = [
                ..AssemblyManager.GetModAssemblies("TerraJS"),
                typeof(ModLoader).Assembly,
                typeof(Vector2).Assembly,
                ..pluginAssemblies
            ];

            var exts = AssemblyUtils.GetExtensionTypes(assemblies);

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

                if (!TerraJS.IsLoading)
                    option.AllowClr(GlobalAPI._ab);
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
