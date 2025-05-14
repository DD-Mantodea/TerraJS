using Terraria.ModLoader;
using Jint;
using System;
using TerraJS.API;
using System.IO;
using Jint.Runtime.Interop;
using System.Reflection;
using System.Dynamic;
using System.Collections.Generic;
using Terraria.ID;

namespace TerraJS
{
	public class TerraJS : Mod
    {
        public static Engine Engine;

        public static TerraJS Instance;

        public static GlobalAPI GlobalAPI;

        public override void Load()
        {
            Instance = this;

            InitializeEngine();

            LoadAllScripts();

            GlobalAPI.Event.InvokeEvent("ModLoad");
        }

        public override void PostSetupContent()
        {
            GlobalAPI.Event.InvokeEvent("PostSetupContent");
        }

        public void InitializeEngine()
        {
            Engine = new Engine();

            GlobalAPI = new();

            BindInstance("TerraJS", GlobalAPI);

            BindStatic("ItemID", typeof(ItemID));

            BindStatic("ItemUseStyleID", typeof(ItemUseStyleID));

            BindProperties("DamageClass", typeof(DamageClass).GetProperties(BindingFlags.Public | BindingFlags.Static));
        }

        public void BindProperties(string name, PropertyInfo[] members)
        {
            dynamic expandoObj = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expandoObj;

            foreach (var property in members)
                expandoDict[property.Name] = property.GetValue(null);

            Engine.SetValue(name, expandoObj);
        }

        public void BindInstance(string name, object instance) => Engine.SetValue(name, instance);

        public void BindStatic(string name, Type type) => Engine.SetValue(name, TypeReference.CreateTypeReference(Engine, type));

        public void LoadAllScripts()
        {
            if (!Directory.Exists("./TerraJS")) Directory.CreateDirectory("./TerraJS");

            if (!Directory.Exists("./TerraJS/Scripts")) Directory.CreateDirectory("./TerraJS/Scripts");

            if (!Directory.Exists("./TerraJS/Textures")) Directory.CreateDirectory("./TerraJS/Textures");

            foreach (var file in Directory.GetFiles("./TerraJS/Scripts"))
            {
                string script = File.ReadAllText(file);

                Engine.Execute(script);
            }
        }

        public override void Unload()
        {
            foreach(var i in GlobalAPI.Event.Events.Values)
                i.Delegates.Clear();

            base.Unload();
        }
    }
}
