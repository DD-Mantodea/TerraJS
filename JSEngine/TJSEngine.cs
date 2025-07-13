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

namespace TerraJS.JSEngine
{
    [HideToJS]
    public class TJSEngine
    {
        public static Engine Engine;

        public static GlobalAPI GlobalAPI = new();

        public static TranslationAPI TranslationAPI = new();

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
                option.CatchClrExceptions(exception =>
                {
                    Console.WriteLine($"[Jint] CLR Exception: {exception.Message}");
                    return true;
                });
            });

            BindingUtils.BindInstance("TJS", GlobalAPI);

            BindingUtils.BindStaticOrEnumOrConst("Cultures", typeof(GameCulture.CultureName));

            BindingUtils.BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));

            BindInnerMethods();

            GlobalAPI.Unload();

            TranslationAPI.Unload();

            LoadScripts();
        }

        public static void BindInnerMethods()
        {
            Engine.SetValue("require", require);
        }

        private static JsValue require(string path)
        {
            return Engine.Evaluate($"importNamespace(\"{path}\")");
        }

        public static void LoadScripts()
        {
            FileUtils.CreateFolderIfNotExist(TerraJS.ModPath);

            FileUtils.CreateFolderIfNotExist(Path.Combine(TerraJS.ModPath, "Scripts"));

            FileUtils.CreateFolderIfNotExist(Path.Combine(TerraJS.ModPath, "Textures"));

            var files = Directory.GetFiles(Path.Combine(TerraJS.ModPath, "Scripts"), "*.js", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string script = File.ReadAllText(file);

                Engine.Execute($"(function() {{ {script} }})()", file);
            }
        }

        public static void Unload()
        {
            GlobalAPI.Unload();

            TranslationAPI.Unload();

            Engine.Dispose();

            BindingUtils.Values.Clear();

            Engine = null;

            GlobalAPI = null;

            TranslationAPI = null;

            TerraJS.Instance.Logger.Info("[Jint] JSEngine unloaded successfully.");
        }
    }
}
