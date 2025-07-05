using System;

namespace TerraJS.Contents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterUIAttribute(string id) : Attribute 
    {
        public string ID = id;
    }
}
