using System;

namespace TerraJS.Contents.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommentAttribute(string comment) : Attribute
    {
        public string Comment = comment;
    }
}
