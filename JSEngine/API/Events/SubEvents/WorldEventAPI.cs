using System;
using System.Collections.Generic;
using TerraJS.Contents.Attributes;
using TerraJS.JSEngine.API.Events.Ref;
using Terraria.WorldBuilding;

namespace TerraJS.JSEngine.API.Events.SubEvents
{
    public class WorldEventAPI : BaseEventAPI
    {
        [HideToJS]
        public Action<List<GenPass>, RefBox<double>> ModifyWorldGenTasksEvent;

        [EventInfo("passes", "totalWeight")]
        public void ModifyWorldGenTasks(Action<List<GenPass>, RefBox<double>> @delegate) => ModifyWorldGenTasksEvent += @delegate;
    }
}
