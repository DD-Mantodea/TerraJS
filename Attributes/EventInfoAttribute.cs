using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventInfoAttribute(string[] paramNames) : Attribute
    {
        public string[] ParameterNames = paramNames;
    }
}
