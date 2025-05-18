using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraJS.API.Quests.QuestGUI;
using TerraJS.Extensions;
using Terraria.ModLoader;

namespace TerraJS.API.Quests.QuestCommands
{
    public class TJSQuest : ModCommand
    {
        public static Dictionary<string, Action> Actions { get; set; } = new();

        public override string Command => "tjsquest";

        public override CommandType Type => CommandType.Chat;

        static TJSQuest()
        {
            Actions.Add("editing_mode", () =>
            {
                QuestPanel.Instance.EditingMode.Reverse();
            });
        }

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            
        }
    }
}
