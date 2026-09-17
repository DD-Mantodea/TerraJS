using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerraJS.Contents.Extensions;
using TerraJS.JSEngine.API.Commands.CommandGUI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.CommandArguments.EntityArguments
{
    public class NPCArgument(string name, bool isOptional = false) : EntityArgument<NPC>(name, isOptional)
    {
        public static List<string> Completions = [.. ContentSamples.NpcsByNetId.Values.Select(n => {
            if (n.ModNPC is null)
                return $"Terraria:{NPCID.Search.GetName(n.type)}";
            return n.ModNPC.FullName.Replace('/', ':');
        })];

        public override List<string> GetAllIdentifiers() => Completions;

        public override bool FromString(string content, object last, out object value)
        {
            value = null;

            if (RegExp.TryMatch(content, out var match))
            {
                var identifier = match.Groups[1].Value.Split(':');

                var modName = identifier[0];

                var npcName = identifier[1];

                if (modName != "Terraria" && !ModLoader.HasMod(modName))
                    return false;

                var npc = modName == "Terraria" ? NPCID.Search.GetId(npcName) : TJSEngine.GlobalAPI.NPC.GetModNPC(modName, npcName);

                if (npc == -1)
                    return false;

                value = NPC.NewNPCDirect(new CommandEntitySource(), 0, 0, npc);

                var datas = match.Groups[2].Value.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

                foreach (var data in datas)
                {
                    var parts = data.Split("=");

                    var type = value.GetType();

                    var f = type.GetField(parts[0], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    var p = type.GetProperty(parts[0], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (f == null && p == null)
                    {
                        Main.NewText($"Field or Property not found: {parts[0]}");

                        return false;
                    }

                    var variType = f == null ? p.PropertyType : f.FieldType;

                    if (EntityAssignment.Assignments.TryGetValue(variType.FullName, out var assignment))
                        assignment(value, f as MemberInfo ?? p, parts[1]);
                }

                return true;
            }

            return false;
        }
    }
}
