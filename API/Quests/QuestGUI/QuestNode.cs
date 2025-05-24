using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.BasicComponents;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.API.Quests.QuestGUI
{
    public class QuestNode : UIElementGroup
    {
        public QuestNode()
        {
            SetSize(64, 64);

            LayoutType = LayoutType.Custom;

            var icon = new UIView().Join(this);

            icon.SetSize(64, 64);

            icon.DrawAction += (gameTime, spriteBatch) =>
            {
                var texture = IsMouseHovering
                ? ModContent.Request<Texture2D>("TerraJS/Textures/UI/Quests/QuestNodeHover").Value
                : ModContent.Request<Texture2D>("TerraJS/Textures/UI/Quests/QuestNode").Value;

                spriteBatch.Draw(texture, icon.GetBounds().LeftTop, Color.White);
            };

            icon.IgnoreMouseInteraction = true;

            var draggable = new SUIDraggableView(this)
            {
                ConstrainInParent = true,
                BackgroundColor = Color.Transparent
            }.Join(this);

            draggable.SetSize(64, 64);
        }

    }
}
