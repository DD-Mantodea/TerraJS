using System;

namespace TerraJS.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterUIAttribute(string id) : Attribute 
    {
        public string ID = id;
    }
}
