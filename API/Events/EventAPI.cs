using Jint;
using Jint.Native;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TerraJS.API.Events.BasicEvents;
using TerraJS.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events
{
    public class EventAPI : BaseAPI
    {
        public Dictionary<string, TJSEvent> Events = new Dictionary<string, TJSEvent>
        {
            ["ModLoad"] = new(),
            ["PostSetupContent"] = new(),
        };

        public ItemEventAPI Item = new();

        public TileEventAPI Tile = new();

        public EventAPI()
        {
            RecipeEvents();
            ItemEvents();
            TileEvents();
            LocalizationEvents();
        }
        public void NewEvent(string name) => Events.Add(name, new TJSEvent());

        public void NewBoolEvent(string name, bool defaultValue) => Events.Add(name, new TJSBoolEvent(defaultValue));
        private void DefaultEventHook(Type hookType, string methodName, string eventName)
        {
            var method = hookType.GetMethod(methodName);

            var args = RegistryUtils.Parameters2Types(method.GetParameters()).ToList();

            if (!method.IsStatic)
                args.Insert(0, hookType);

            string actionTypeName = args.Count switch
            {
                0 => "System.Action",
                _ => $"System.Action`{args.Count}"
            };

            var bindType = Type.GetType(actionTypeName).MakeGenericType([.. args]);

            var invokeEvent = GetType().GetMethod("InvokeEvent", BindingFlags.NonPublic | BindingFlags.Instance);

            var argsExpr = new List<Expression>();

            var paramsExpr = new List<ParameterExpression>();

            for (int i = 0; i < args.Count; i++)
            {
                var parameter = Expression.Parameter(args[i]);

                if (!args[i].IsValueType)
                    argsExpr.Add(parameter);
                else
                {
                    argsExpr.Add(Expression.Convert(parameter, typeof(object)));
                }

                paramsExpr.Add(parameter);
            }

            var argArrayExpr = Expression.NewArrayInit(typeof(object), argsExpr);

            var globalAPI = Expression.Field(null, typeof(TerraJS), "GlobalAPI");

            var eventAPI = Expression.Field(globalAPI, typeof(GlobalAPI), "Event");

            var callExpr = Expression.Call(eventAPI, invokeEvent, Expression.Constant(eventName), argArrayExpr);

            var lambdaExpr = Expression.Lambda(callExpr, paramsExpr);

            var str = lambdaExpr.ToString();

            var @delegate = lambdaExpr.Compile();

            var del = @delegate.Method.CreateDelegate(bindType);

            MonoModHooks.Add(method, @delegate);
        }

        private void RecipeEvents()
        {
            NewEvent("AddRecipes");
            NewEvent("PostAddRecipes");
        }

        private void ItemEvents()
        {
            Item.NewEvent("UpdateInventory");
            Item.NewEvent("SetDefaults");
            Item.NewEvent("RightClick");
            Item.NewBoolEvent("CanRightClick", false);
            Item.NewBoolEvent("UseItem", null);
            Item.NewBoolEvent("CanUseItem", true);
            Item.NewBoolEvent("ConsumeItem", true);
        }

        private void TileEvents()
        {
            Tile.NewBoolEvent("CanPlaceTile", true);
            Tile.NewEvent("PlaceTile");
            Tile.NewBoolEvent("CanBreakTile", true);
            Tile.NewEvent("BreakTile");
        }

        private void LocalizationEvents()
        {
            NewEvent("TranslationModify");
        }

        public void InvokeEvent(string eventName, params object[] args)
        {
            if (!Events.TryGetValue(eventName, out TJSEvent @event)) return;

            @event.Invoke(args);
        }

        public bool? InvokeBoolEvent(string eventName, params object[] args)
        {
            if (!Events.TryGetValue(eventName, out TJSEvent @event)) 
                return false;

            if(@event is TJSBoolEvent boolEvent)
                return boolEvent.Invoke(args);

            return false;
        }

        public void OnEvent(string eventName, Delegate @delegate)
        {
            if (!Events.TryGetValue(eventName, out TJSEvent @event)) return;

            @event.AddEventHandler(@delegate);
        }

        internal override void Reload()
        {
            foreach(var @event in Events.Values)
            {
                @event.ClearEventHandlers();
            }

            Item.Reload();
            Tile.Reload();
        }
    }

    public abstract class SubEventAPI : BaseAPI
    {
        public Dictionary<string, TJSEvent> Events = [];

        public void InvokeEvent(string eventName, params object[] args)
        {
            if (!Events.TryGetValue(eventName, out var @event)) return;

            @event.Invoke(args);
        }

        public bool? InvokeBoolEvent(string eventName, params object[] args)
        {
            if (!Events.TryGetValue(eventName, out TJSEvent @event))
                return false;

            if (@event is TJSBoolEvent boolEvent)
                return boolEvent.Invoke(args);

            return false;
        }

        public void OnEvent(string eventName, Delegate @delegate)
        {
            if (!Events.TryGetValue(eventName,out var @event)) return;

            @event.AddEventHandler(@delegate);
        }

        public void NewEvent(string name) => Events.Add(name, new TJSEvent());

        public void NewBoolEvent(string name, bool? defaultValue) => Events.Add(name, new TJSBoolEvent(defaultValue));

        internal override void Reload()
        {
            foreach (var @event in Events.Values)
            {
                @event.ClearEventHandlers();
            }
        }
    }

    public class ItemEventAPI : SubEventAPI { }

    public class TileEventAPI : SubEventAPI { }
}
