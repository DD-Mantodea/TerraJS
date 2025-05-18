using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;

namespace TerraJS.API.Quests.QuestGUI
{
    public class QuestNode : UIElementGroup
    {
        public QuestNode()
        {
            BackgroundColor = Color.Green;

            var draggable = new SUIDraggableView(this)
            {
                ConstrainInParent = true,
                BackgroundColor = Color.Transparent
            }.Join(this);

            draggable.SetSize(0, 0, 1, 1);
        }
    }
}
