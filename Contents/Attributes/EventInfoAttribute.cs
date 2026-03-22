using System;

namespace TerraJS.Contents.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventInfoAttribute(params string[] paramNames) : Attribute
    {
        public string[] ParameterNames = paramNames;
    }
}
