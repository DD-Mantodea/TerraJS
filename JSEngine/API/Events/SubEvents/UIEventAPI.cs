using System;
using TerraJS.Contents.Attributes;

namespace TerraJS.JSEngine.API.Events.SubEvents
{
    public class UIEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action RegisterUIEvent;

        [EventInfo]
        public void RegisterUI(Action @delegate) => RegisterUIEvent += @delegate;
    }
}
