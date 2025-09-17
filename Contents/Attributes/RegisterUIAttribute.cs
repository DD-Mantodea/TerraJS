using System;

namespace TerraJS.Contents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterUIAttribute(string id, string layer = "TerraJS: Player Chat", int priority = 0) : Attribute 
    {
        public string ID = id;

        public string Layer = layer;

        public int Priority = priority;
    }
}
