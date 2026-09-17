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
    public class DetectorType : DetectorObject
    {
        public DetectorModule Module;

        public DetectorType(Type type, DetectorModule module)
        {
            Type = type;

            Module = module;

            foreach (var i in Type.GetMembers())
                AddMember(i);
        }

        public Type Type;

        public List<DetectorMember> Members = [];

        public override string Serialize()
        {
            var ret = new StringBuilder();

            var typeDef = Type.IsInterface ? "interface" : "class";

            if (Type.IsAbstract)
                typeDef = "abstract " + typeDef;

            var typeContent = $"export {typeDef} {Type2ClassName(Type)}";

            if (Type.BaseType != null)
                typeContent += $" extends {Type2ClassName(Type.BaseType)}";

            var interfaces = Type.GetInterfaces();

            if (interfaces.Length != 0)
                typeContent += $" implements {string.Join(" ,", interfaces.Select(i => Type2ClassName(i)))}";

            typeContent += " {";

            ret.AppendLine(typeContent);

            foreach (var i in Members)
            {
                var ser = i.Serialize();

                if(ser != "")
                    ret.AppendLine(ser);
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

            if (info is ConstructorInfo && Type.IsStatic())
                return;

            if (info is MethodInfo method)
            {
                if (method.IsIllegal())
                    return;

                if (method.GetCustomAttribute<ExtensionAttribute>() != null)
                {
                    var targetType = method.GetParameters().First().ParameterType;

                    if (targetType.IsGenericParameter)
                    {
                        if (Detector.GenericExtensionMethods.TryGetValue(targetType, out var extMethods))
                            extMethods.Add(method);
                        else
                        {
                            var exts = new List<MethodInfo> { method };

                            Detector.GenericExtensionMethods.TryAdd(targetType, exts);
                        }
                    }
                    else
                    {
                        if (Detector.ExtensionMethods.TryGetValue(targetType, out var extMethods))
                            extMethods.Add(method);
                        else
                        {
                            var exts = new List<MethodInfo> { method };

                            Detector.ExtensionMethods.TryAdd(targetType, exts);
                        }
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
                else if (info is ConstructorInfo constructorInfo)
                {
                    var target = Members.First(t => t.MemberInfo.Name == info.Name).MemberInfo as ConstructorInfo;

                    var infoParams = RegistryUtils.Parameters2Types(constructorInfo.GetParameters());

                    var targetParams = RegistryUtils.Parameters2Types(target.GetParameters());

                    if (infoParams.SequenceEqual(targetParams))
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

            Members.TryAdd(new(info, Type));
        }

        public void AddExtensionMethods()
        {   
            if (Detector.ExtensionMethods.TryGetValue(Type, out var extMethods))
            {
                foreach (var method in extMethods)
                {
                    Members.TryAdd(new(method, Type));

                    Module.AddImport(method.ReturnType);

                    foreach (var p in method.GetParameters())
                        Module.AddImport(p.ParameterType);

                    if (method.IsGenericMethod)
                        Module.AddImport(typeof(Type));
                }
            }

            foreach (var genericType in Detector.GenericExtensionMethods.Keys)
            {
                if (CheckConstraints(Type, genericType))
                {
                    foreach (var method in Detector.GenericExtensionMethods[genericType])
                    {
                        Members.TryAdd(new(method, Type));

                        Module.AddImport(method.ReturnType);

                        foreach (var p in method.GetParameters())
                            Module.AddImport(p.ParameterType);

                        if (method.IsGenericMethod)
                            Module.AddImport(typeof(Type));
                    }
                }
            }
        }

        public bool CheckConstraints(Type targetType, Type genericType)
        {
            var constraints = genericType.GetGenericParameterConstraints();

            var attributes = genericType.GenericParameterAttributes;

            var specialMask = attributes & GenericParameterAttributes.SpecialConstraintMask;

            foreach (var c in constraints)
            {
                if (c.IsInterface)
                    if (!c.IsAssignableFrom(targetType))
                        return false;
                else
                    if (!targetType.IsSubclassOf(c)) 
                        return false;
            }

            if (specialMask.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                if (!targetType.IsClass)
                    return false;

            if (specialMask.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                if (!targetType.IsValueType)
                    return false;

            if (specialMask.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                if (targetType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, []) == null)
                    return false;

            return true;
        }

        public override bool Equals(object obj) => obj is DetectorType clazz && clazz.Type == Type;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
