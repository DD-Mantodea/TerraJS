using System;
using TerraJS.API;
using TerraJS.Contents.UI;
using TerraJS.JSEngine.API.UIs.Components.Containers;

namespace TerraJS.JSEngine.API.UIs
{
    public class UIAPI : BaseAPI
    {
        public JsContainer RegisterUI(string ID, string layer)
        {
            if (UISystem.UIInstances.TryGetValue(ID, out _))
                return null;

            var container = new JsContainer { ID = ID };

            if (UISystem.TJSUILayers.TryGetValue(layer, out var uiLayer))
            {
                uiLayer.Register(container);

                return container;
            }

            return null;
        }

        public JsContainer GetUI(string ID)
        {
            if (!UISystem.UIInstances.TryGetValue(ID, out var ret))
                return null;

            if (ret is JsContainer c)
                return c;

            return null;
        }

        internal override void Unload()
        {
            
        }
    }

    public class LayerID
    {
        public const string PlayerChat = "TerraJS: Player Chat";
    }
}
