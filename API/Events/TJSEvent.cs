using Jint;
using Jint.Native;
using Microsoft.Build.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerraJS.API.Events
{
    public class TJSEvent
    {
        public List<Delegate> Delegates = [];

        public bool Custom = false;

        public void Invoke(params object[] args)
        {
            if (Delegates.Count == 0) 
                return;

            try
            {
                var jsArgs = args.Select((obj, i) => JsValue.FromObject(TerraJS.Engine, obj)).ToArray();

                foreach (var @delegate in Delegates)
                    @delegate.DynamicInvoke(JsValue.Undefined, jsArgs);
            }
            catch
            {

            }
        }

        public void AddEventHandler(Delegate @delegate)
        {
            Delegates.Add(@delegate);
        }

        public void ClearEventHandlers() => Delegates.Clear();
    }

    public abstract class TJSEvent<T> : TJSEvent where T : struct
    {
        public new abstract T? Invoke(params object[] args);
        public new virtual void AddEventHandler(Delegate @delegate)
        {
            Delegates.Add(@delegate);
        }
    }
}
