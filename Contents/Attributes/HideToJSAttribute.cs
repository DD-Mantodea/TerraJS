using System;

namespace TerraJS.Contents.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class HideToJSAttribute : Attribute
    {

    }
}
