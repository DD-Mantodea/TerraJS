﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Runtime.Interop;

namespace TerraJS.Contents.Utils
{
    public class BindingUtils
    {
        public static List<(string, object)> Values = [];

        public static void BindInstance(string name, object instance)
        {
            TerraJS.Engine.SetValue(name, instance);
            Values.Add((name, instance));
        }

        public static void BindProperties(string name, PropertyInfo[] members)
        {
            dynamic expandoObj = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)expandoObj;

            foreach (var property in members)
                expandoDict[property.Name] = property.GetValue(null);

            TerraJS.Engine.SetValue(name, expandoObj);

            Values.Add((name, expandoObj));
        }

        public static void BindStaticOrEnumOrConst(string name, Type type)
        {
            TerraJS.Engine.SetValue(name, TypeReference.CreateTypeReference(TerraJS.Engine, type));
            Values.Add((name, type));
        }
    }
}
