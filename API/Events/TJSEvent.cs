using Jint;
using Jint.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerraJS.API.Events
{
    public class TJSEvent
    {
        public List<JsValue> Delegates = [];

        public bool Custom = false;

        public void Invoke(params object[] args)
        {
            if (Delegates.Count == 0) return;

            try
            {
                var jsArgs = args.Select((obj, i) => JsValue.FromObject(TerraJS.Engine, obj)).ToArray();

                foreach (var jsFunc in Delegates)
                    jsFunc.Call(JsValue.Undefined, jsArgs);
            }
            catch
            {

            }
        }

        public void AddEventHandler(JsValue jsValue)
        {
            Delegates.Add(jsValue);
        }
    }

    public class TJSEvent<T> : TJSEvent where T : Delegate
    {

    }
}
