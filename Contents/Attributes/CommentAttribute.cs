using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.Contents.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommentAttribute(string comment) : Attribute
    {
        public string Comment = comment;
    }
}
