using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Events;
using TerraJS.Extensions;
using Terraria;

namespace TerraJS.DetectorJS.DetectorObjects
{
    public class DetectorClass(Type type) : DetectorObject
    {
        public Type Type = type;

        public List<DetectorMember> Members = [];

        public override string Serialize()
        {
            var ret = new StringBuilder();

            ret.AppendLine($"export class {Type2ClassName(Type)}" + (Type.BaseType == null ? "" : $" extends {Type2ClassName(Type.BaseType)}") + " {");

            foreach (var i in Type.GetMembers())
                AddMember(i);

            foreach (var i in Members)
            {
                if(i.Serialize() != "")
                    ret.AppendLine(i.Serialize());
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

            if (info is MethodInfo method && method.IsIllegal())
                return;

            if (Members.Exists(t => t.MemberInfo.Name == info.Name))
            {
                var target = Members.First(t => t.MemberInfo.Name == info.Name);

                if (target.MemberInfo.DeclaringType != Type)
                    target.MemberInfo = info;

                return;
            }

            Members.TryAdd(new(info));
        }

        public override bool Equals(object obj) => obj is DetectorClass clazz && clazz.Type == Type;
    }
}
