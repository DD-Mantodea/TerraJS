using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
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
