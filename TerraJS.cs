using Terraria.ModLoader;
using System;
using TerraJS.API;
using System.IO;
using System.Reflection;
using Terraria.Localization;
using System.Linq;
using Terraria;
using TerraJS.API.Commands.CommandArguments.BasicArguments;
using TerraJS.DetectorJS;
using TerraJS.Assets.Managers;
using TerraJS.Contents.Attributes;
using TerraJS.JSEngine;
using Terraria.ID;
using NetSimplified;
using TerraJS.Assets.AssetManagers;
using MonoMod.RuntimeDetour;
using System.Reflection.Emit;
using System.Collections.Generic;
using MonoMod.Utils;
using log4net;
using Jint;
using Acornima;
using System.Threading.Tasks;
using TerraJS.JSEngine.Plugins;
using System.Threading;

namespace TerraJS
{
	public class TerraJS : Mod
    {
        public static TerraJS Instance;

        public static FontManager FontManager = new();

        public static TextureManager TextureManager = new();

        public static SHAManager SHAManager = new();

        public static bool IsLoading = false;

        [HideToJS]
        public override void Load()
        {
            var eventInfo = typeof(DetourManager).GetEvent("DetourApplied");

            var eventField = typeof(DetourManager).GetField("DetourApplied", BindingFlags.NonPublic | BindingFlags.Static);

            if (eventField.GetValue(null) is Action<DetourInfo> action)
                foreach (var @delegate in action.GetInvocationList())
                    eventInfo.RemoveEventHandler(null, @delegate);

            DetourManager.DetourApplied += DetourManager_DetourApplied;

            Instance = this;

            IsLoading = true;

            TJSEngine.LoadPlugins();

            PluginRegistryInfos.RegisterAll();

            TJSEngine.Load();

            TJSEngine.GlobalAPI.Event.ModLoadEvent?.Invoke();

            RegisterCommands();

            /*
            MonoModHooks.Add(typeof(UserInterface).GetMethod("Update"), ModifyUpdate);

            MonoModHooks.Add(typeof(UserInterface).GetMethod("Draw"), ModifyDraw);

            MonoModHooks.Add(typeof(GameInterfaceLayer).GetMethod("Draw"), ModifyDrawLayer);
            */

            AddContent<NetModuleLoader>();
        }

        private static void DetourManager_DetourApplied(DetourInfo info)
        {
            var owner = info.Entry.DeclaringType?.Assembly ??
                (info.Entry is DynamicMethod method
                ? (typeof(DynamicMethod).GetField("_module", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(method) as Module)?.Assembly
                : null);

            if (owner == null)
                return;

            var monoModHooks = typeof(MonoModHooks);

            var list = monoModHooks.GetMethod("GetDetourList", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [owner]);

            (monoModHooks.GetNestedType("DetourList", BindingFlags.NonPublic).GetField("detours").GetValue(list) as List<DetourInfo>).Add(info);

            var msg = $"Hook {monoModHooks.GetMethod("StringRep", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [info.Method.Method])} added by {owner.GetName().Name}";

            var targetSig = MethodSignature.ForMethod(info.Method.Method);
            var detourSig = MethodSignature.ForMethod(info.Entry, ignoreThis: true);

            if (detourSig.ParameterCount != targetSig.ParameterCount + 1 || detourSig.FirstParameter.GetMethod("Invoke") is null)
                msg += " WARNING! No orig delegate, incompatible with other hook to this method";

            (typeof(Logging).GetProperty("tML", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as ILog).Debug(msg);
        }

        [HideToJS]
        public static void Reload()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                Main.NewText("请在单人模式下重载 TerraJS", 255, 0, 0);
                return;
            }

            FontManager.Load();

            TextureManager.Load();

            SHAManager.Load();

            TJSEngine.Load();

            TJSEngine.GlobalAPI.Event.InGameReloadEvent?.Invoke();

            Main.NewText("重载完成");
        }

        [HideToJS]
        public void RegisterCommands()
        {
            /*
            GlobalAPI.Command.CreateCommandRegistry("tjsquest")
                .NextArgument(new ComboArgument("value", ["edit"]))
                .Execute((g, _) =>
                {
                    var value = g.GetString("value");

                    switch (value)
                    {
                        case "edit":
                            QuestPanel.Instance.EditingMode = !QuestPanel.Instance.EditingMode;
                            break;
                    }
                })
                .Register();
            */

            TJSEngine.GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "reload"))
                .Execute((_, _) => Reload())
                .Register();

            TJSEngine.GlobalAPI.Command.CreateCommandRegistry("terrajs")
                .NextArgument(new ConstantArgument("feature", "detect"))
                .Execute((_, _) =>
                {
                    var thread = new Thread(Detector.Detect);

                    thread.Start();
                })
                .Register();
        }

        [HideToJS]
        public override void PostSetupContent()
        {
            FontManager.Load();

            TextureManager.Load();

            SHAManager.Load();

            TJSEngine.GlobalAPI.Event.PostSetupContentEvent?.Invoke(); 
            
            IsLoading = false;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetModule.ReceiveModule(reader, whoAmI);
        }


        public override object Call(params object[] args)
        {
            if (args.Length == 0) return null;

            return null;
        }

        /*
        public void ModifyUpdate(Action<UserInterface, GameTime> orig, UserInterface ui, GameTime time)
        {
            if (QuestPanel.Instance != null && QuestPanel.Instance.Enabled) 
                return;

            orig(ui, time);
        }

        public void ModifyDraw(Action<UserInterface, SpriteBatch, GameTime> orig, UserInterface ui, SpriteBatch spriteBatch, GameTime time)
        {
            if (QuestPanel.Instance != null && QuestPanel.Instance.Enabled)
                return;

            orig(ui, spriteBatch, time);
        }

        public bool ModifyDrawLayer(Func<GameInterfaceLayer, bool> orig, GameInterfaceLayer layer)
        {
            if (layer.Name.StartsWith("Vanilla") && layer.Name != "Vanilla: Cursor" && QuestPanel.Instance != null && QuestPanel.Instance.Enabled)
                return true;

            return orig(layer);
        }
        */

        [HideToJS]
        public override void Unload()
        {
            TJSEngine.Unload();

            var eventInfo = typeof(DetourManager).GetEvent("DetourApplied");

            var eventField = typeof(DetourManager).GetField("DetourApplied", BindingFlags.NonPublic | BindingFlags.Static);

            if (eventField.GetValue(null) is Action<DetourInfo> action)
                foreach (var @delegate in action.GetInvocationList())
                    eventInfo.RemoveEventHandler(null, @delegate);

            DetourManager.DetourApplied += (info) =>
            {
                var owner = info.Entry.DeclaringType.Assembly;

                if (owner == null)
                    return;

                var monoModHooks = typeof(MonoModHooks);

                var list = monoModHooks.GetMethod("GetDetourList", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [owner]);

                (monoModHooks.GetNestedType("DetourList", BindingFlags.NonPublic).GetField("detours").GetValue(list) as List<DetourInfo>).Add(info);

                var msg = $"Hook {monoModHooks.GetMethod("StringRep", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [info.Method.Method])} added by {owner.GetName().Name}";

                var targetSig = MethodSignature.ForMethod(info.Method.Method);
                var detourSig = MethodSignature.ForMethod(info.Entry, ignoreThis: true);

                if (detourSig.ParameterCount != targetSig.ParameterCount + 1 || detourSig.FirstParameter.GetMethod("Invoke") is null)
                    msg += " WARNING! No orig delegate, incompatible with other hook to this method";

                (typeof(Logging).GetProperty("tML", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as ILog).Debug(msg);
            };

            base.Unload();
        }
    }
}
