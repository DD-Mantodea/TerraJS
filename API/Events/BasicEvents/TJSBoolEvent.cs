using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Native;

namespace TerraJS.API.Events.BasicEvents
{
    public class TJSBoolEvent(bool? defaultValue) : TJSEvent<bool>
    {
        private bool? _defaultValue = defaultValue;

        public override bool? Invoke(params object[] args)
        {
            if (Delegates.Count == 0)
                return _defaultValue;

            try
            {
                var results = new List<bool>();

                var jsArgs = args.Select((obj, i) => JsValue.FromObject(TerraJS.Engine, obj)).ToArray();

                foreach (var @delegate in Delegates)
                    results.Add((@delegate.DynamicInvoke(JsValue.Undefined, jsArgs) as JsValue).AsBoolean());

                return !results.Contains(false);
            }
            catch
            {
                return _defaultValue;
            }
        }
    }
}
