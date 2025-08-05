using TerraJS.API;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Components.Containers;

namespace TerraJS.JSEngine.API.UIs
{
    public class UIAPI : BaseAPI
    {
        public TJSUIContainer RegisterUI(string ID)
        {
            if (TJSUISystem.Layer.TryGetUIByID(ID, out _))
                return null;

            var container = new TJSUIContainer { ID = ID };

            TJSUISystem.Layer.Register(container);

            return container;
        }

        public TJSUIContainer GetUI(string ID)
        {
            if (!TJSUISystem.Layer.TryGetUIByID(ID, out var ret))
                return null;

            if (ret is TJSUIContainer c)
                return c;

            return null;
        }

        internal override void Unload()
        {
            
        }
    }
}
