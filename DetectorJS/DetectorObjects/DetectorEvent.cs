using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorEvent(EventInfo @event) : DetectorObject
    {
        public EventInfo Event = @event;

        public override string Serialize()
        {
            return "";
        }
    }
}
