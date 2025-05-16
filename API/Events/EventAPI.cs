using Jint;
using Jint.Native;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TerraJS.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Events
{
    public class EventAPI
    {
        public Dictionary<string, TJSEvent> Events = new Dictionary<string, TJSEvent>
        {
            ["ModLoad"] = new TJSEvent(),
            ["PostSetupContent"] = new TJSEvent()
        };

        public SubEventAPI Item = new();

        public EventAPI()
        {
            RecipeEvents();
            ItemEvents();
            LocalizationEvents();
        }

        private void NewEvent<T>(string name) where T : Delegate => Events.Add(name, new TJSEvent<T>());

        private void NewEvent(string name) => Events.Add(name, new TJSEvent());
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

            var bindType = Type.GetType(actionTypeName).MakeGenericType(args.ToArray());

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
        }

        private void LocalizationEvents()
        {
            NewEvent("TranslationModify");
        }

        public void InvokeEvent(string eventName, params object[] args)
        {
            if (!Events.ContainsKey(eventName)) return;

            var @event = Events[eventName];

            @event.Invoke(args);
        }

        public void OnEvent(string eventName, Delegate @delegate)
        {
            if (!Events.ContainsKey(eventName)) return;

            var @event = Events[eventName];

            @event.AddEventHandler(JsValue.FromObject(TerraJS.Engine, @delegate));
        }
    }

    public class SubEventAPI
    {
        public Dictionary<string, TJSEvent> Events = new Dictionary<string, TJSEvent>();

        public void InvokeEvent(string eventName, params object[] args)
        {
            if (!Events.ContainsKey(eventName)) return;

            var @event = Events[eventName];

            @event.Invoke(args);
        }

        public void OnEvent(string eventName, Delegate @delegate)
        {
            if (!Events.ContainsKey(eventName)) return;

            var @event = Events[eventName];

            @event.AddEventHandler(JsValue.FromObject(TerraJS.Engine, @delegate));
        }

        public void NewEvent(string name) => Events.Add(name, new TJSEvent());
    }
}
