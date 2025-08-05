using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using Terraria;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorClass : DetectorObject
    {
        public DetectorClass(Type type)
        {
            Type = type;

            foreach (var i in Type.GetMembers())
                AddMember(i);
        }

        public Type Type;

        public List<DetectorMember> Members = [];

        public override string Serialize()
        {
            var ret = new StringBuilder();

            ret.AppendLine($"export class {Type2ClassName(Type)}" + (Type.BaseType == null ? "" : $" extends {Type2ClassName(Type.BaseType)}") + " {");

            if (Detector.ExtensionMethods.TryGetValue(Type, out var extMethods))
                Members.TryAddRange([.. extMethods.Select(m => new DetectorMember(m))]);

            foreach (var i in Members)
            {
                if(i.Serialize() != "")
                    ret.AppendLine(i.Serialize());
            }

            if (typeof(IEnumerable).IsAssignableFrom(Type))
            {
                var t = Type.GetInterfaces().Append(Type)
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                if (t != null)
                    ret.AppendLine($"[Symbol.iterator](): Iterator<{Type2ClassName(t.GetGenericArguments()[0])}>");
            }

            ret.AppendLine("}");

            return ret.ToString();
        }

        public void AddMember(MemberInfo info)
        {
            if (info is PropertyInfo property && property.PropertyType.IsByRef)
                return;

            if (info is FieldInfo field && field.FieldType.IsByRef)
                return;

            if (info is MethodInfo method)
            {
                if (method.IsIllegal())
                    return;

                if (method.GetCustomAttribute<ExtensionAttribute>() != null)
                {
                    var targetType = method.GetParameters().First().ParameterType;

                    if (Detector.ExtensionMethods.TryGetValue(targetType, out var extMethods))
                        extMethods.Add(method);
                    else
                    {
                        var exts = new List<MethodInfo> { method };

                        Detector.ExtensionMethods.TryAdd(targetType, exts);
                    }

                    return;
                }
            }

            if (Members.Exists(t => t.MemberInfo.Name == info.Name))
            {
                if (info is MethodInfo methodInfo)
                {
                    var target = Members.First(t => t.MemberInfo.Name == info.Name).MemberInfo as MethodInfo;

                    var infoParams = RegistryUtils.Parameters2Types(methodInfo.GetParameters());

                    var targetParams = RegistryUtils.Parameters2Types(target.GetParameters());

                    if (infoParams.SequenceEqual(targetParams) && methodInfo.ReturnType == target.ReturnType)
                        return;
                }
                else
                {
                    var target = Members.First(t => t.MemberInfo.Name == info.Name);

                    if (target.MemberInfo.DeclaringType != Type)
                        target.MemberInfo = info;

                    return;
                }
            }

            Members.TryAdd(new(info));
        }

        public override bool Equals(object obj) => obj is DetectorClass clazz && clazz.Type == Type;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
