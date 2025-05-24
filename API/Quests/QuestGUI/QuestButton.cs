using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraJS.API.Quests.QuestGUI
{
    [RegisterUI("Vanilla: Radial Hotbars", "TerraJS: QuestButton")]
    public class QuestButton : BasicBody
    {
        protected override void OnInitialize()
        {
            SetSize(30, 30);

            IgnoreMouseInteraction = true;

            var button = new UIElementGroup()
            {
                LayoutType = LayoutType.Flexbox,
                FlexDirection = FlexDirection.Row,
                MainAlignment = MainAlignment.Start,
                CrossAlignment = CrossAlignment.Center,
                CrossContentAlignment = CrossContentAlignment.Center,
                BackgroundColor = Color.Transparent
            }.Join(this);
            button.SetSize(30, 30);

            var icon = new UIView().Join(button);
            icon.SetSize(30, 30);
            icon.DrawAction += (gameTime, spriteBatch) =>
            {
                var texture = IsMouseHovering
                ? ModContent.Request<Texture2D>("TerraJS/Textures/UI/Quests/QuestButtonHover").Value
                : ModContent.Request<Texture2D>("TerraJS/Textures/UI/Quests/QuestButton").Value;

                spriteBatch.Draw(texture, icon.GetBounds().LeftTop, Color.White);
                
                if(IsMouseHovering)
                    Main.instance.MouseText(TranslationAPI.LocalizedTexts["Mods.TerraJS.QuestButton.HoverText"], 0, 0);
            };

            icon.LeftMouseClick += (e, args) =>
            {
                QuestPanel.Instance.Enabled = true;
                Main.playerInventory = false;
            };

            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            SetLeft(570);
            SetTop(274);
        }

        public override bool Enabled => Main.playerInventory && !QuestPanel.Instance.Enabled;
    }
}
