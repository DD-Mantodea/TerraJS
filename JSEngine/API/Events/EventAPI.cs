using System;
using System.Linq;
using System.Reflection;
using TerraJS.API.Events.SubEvents;
using TerraJS.Contents.Attributes;
using TerraJS.JSEngine.API.Events.SubEvents;

namespace TerraJS.API.Events
{
    public class BaseEventAPI : BaseAPI
    {
        internal override void Unload()
        {
            GetType().GetFields().Where(f => f.FieldType.IsSubclassOf(typeof(Delegate)))
                .ToList().ForEach(f => f.SetValue(this, null));
        }
    }

    public class EventAPI : BaseEventAPI
    {
        public ItemEventAPI Item = new();

        public TileEventAPI Tile = new();

        public RecipeEventAPI Recipe = new();

        public NPCEventAPI NPC = new();

        public PlayerEventAPI Player = new();

        public UIEventAPI UI = new();

        [HideToJS]
        public Action ModLoadEvent;

        [HideToJS]
        public Action InGameReloadEvent;

        [HideToJS]
        public Action PostSetupContentEvent;

        [EventInfo]
        public void ModLoad(Action @delegate) => ModLoadEvent += @delegate;

        [EventInfo]
        public void InGameReload(Action @delegate) => InGameReloadEvent += @delegate;

        [EventInfo]
        public void PostSetupContent(Action @delegate) => PostSetupContentEvent += @delegate;

        internal override void Unload()
        {
            base.Unload();

            GetType().GetFields().Where(f => f.FieldType.IsSubclassOf(typeof(BaseEventAPI)))
                .ToList().ForEach(f => (f.GetValue(this) as BaseEventAPI).Unload());
        }
    }
}
