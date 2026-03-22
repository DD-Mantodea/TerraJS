using System;
using System.Reflection;

namespace TerraJS.Contents.Extensions
{
    public static class ObjectExt
    {
        public static object GetClone(this object obj)
        {
            if (obj is string || obj.GetType().IsValueType)
                return obj;

            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    field.SetValue(retval, field.GetValue(obj).GetClone());
                }
                catch { }
            }

            return retval;
        }
    }
}
