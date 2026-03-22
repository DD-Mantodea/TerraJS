using System.Reflection.Emit;
using System.Reflection;
using TerraJS.JSEngine.API.UIs;
using TerraJS.JSEngine.API.Translations;
using TerraJS.JSEngine.API.Debuggers;
using TerraJS.JSEngine.API.DotNets;
using TerraJS.JSEngine.API.Items;
using TerraJS.JSEngine.API.Players;
using TerraJS.JSEngine.API.Events;
using TerraJS.JSEngine.API.Commands;
using TerraJS.JSEngine.API.Recipes;
using TerraJS.JSEngine.API.Hooks;
using TerraJS.JSEngine.API.Tiles;
using TerraJS.JSEngine.API.NPCs;
using TerraJS.JSEngine.API.Projectiles;

namespace TerraJS.JSEngine.API
{
    public class GlobalAPI : BaseAPI
    {
        public CommandAPI Command = new();

        public EventAPI Event = new();

        public ItemAPI Item = new();

        public RecipeAPI Recipe = new();

        public ProjectileAPI Projectile = new();

        public TileAPI Tile = new();

        public PlayerAPI Player = new();

        public HookAPI Hook = new();

        public TranslationAPI Translation = new();

        public UIAPI UI = new();

        public NPCAPI NPC = new();

        public DebuggerAPI Debugger = new();

        public DotNetAPI DotNet = new();

        internal static AssemblyName _an = new AssemblyName("TJSContents");

        internal static AssemblyBuilder _ab = AssemblyBuilder.DefineDynamicAssembly(_an, AssemblyBuilderAccess.RunAndCollect);

        internal static ModuleBuilder _mb = _ab.DefineDynamicModule(_an.Name);

        public void Info(object obj) => TerraJS.Instance.Logger.Info(obj);

        public void Debug(object obj) => TerraJS.Instance.Logger.Debug(obj);

        public void Warn(object obj) => TerraJS.Instance.Logger.Warn(obj);

        internal override void Unload()
        {
            foreach (var i in GetType().GetFields())
            {
                if (i.FieldType.IsSubclassOf(typeof(BaseAPI)))
                    (i.GetValue(this) as BaseAPI).Unload();
            }
        }
    }
}
