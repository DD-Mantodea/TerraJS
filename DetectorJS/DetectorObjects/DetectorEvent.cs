using System.Reflection;

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
