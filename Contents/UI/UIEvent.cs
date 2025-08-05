using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.Contents.UI.Components;

namespace TerraJS.Contents.UI
{
    public class UIEvent
    {
        public Dictionary<string, Action<Component>> Listeners = [];

        public void AddListener(string name, Action<Component> listener) => Listeners.TryAdd(name, listener);

        public void RemoveListener(string name) => Listeners.Remove(name);

        public void Invoke(Component c)
        {
            foreach (var listener in Listeners.Values)
                listener.Invoke(c);
        }
    }
}
