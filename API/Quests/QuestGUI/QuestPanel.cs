using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using SilkyUIFramework.Graphics2D;
using Terraria;

namespace TerraJS.API.Quests.QuestGUI
{
    [RegisterUI("Vanilla: Radial Hotbars", "TerraJS: QuestPanel", int.MaxValue)]
    public class QuestPanel : BasicBody
    {
        public static QuestPanel Instance = null;

        public QuestPanel() => Instance = this;

        public bool EditingMode = false;

        public UIElementGroup LeftGroupBar;

        public UIElementGroup RightGroupBar;

        public QuestViewer QuestViewer;

        protected override void OnInitialize()
        {
            SetSize(0, 0, 1, 1);

            SetGap(0);

            FlexDirection = FlexDirection.Row;

            OccupyPlayerInput = true;

            AvailableItem = false;

            Enabled = false;

            LeftGroupBar = new UIElementGroup()
            {
                BackgroundColor = Color.DarkGray * 0.5f,
                BorderColor = Color.Black,
                Border = 2,
            }.Join(this);

            LeftGroupBar.SetSize(0, 0, widthPercent: 0.1f, heightPercent: 1f);

            QuestViewer = new QuestViewer().Join(this);

            QuestViewer.Initialize();

            QuestViewer.SetSize(0, 0, 0.8f, 1f);

            RightGroupBar = new UIElementGroup()
            {
                BackgroundColor = Color.DarkGray * 0.5f,
                BorderColor = Color.Black,
                Border = 2,
            }.Join(this);

            RightGroupBar.SetSize(0, 0, widthPercent: 0.1f, heightPercent: 1f);
        }

        protected override void UpdateStatus(GameTime gameTime)
        {
            RectangleRender.ShadowColor = SUIColor.Border * 0.15f; // HoverTimer.Lerp(0f, 0.25f);
            RectangleRender.ShadowSize = 0f; // HoverTimer.Lerp(0f, 20f);
            RectangleRender.ShadowBlurSize = 0f; // HoverTimer.Lerp(0f, 20f);

            base.UpdateStatus(gameTime);
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (BlurMakeSystem.BlurAvailable)
            {
                if (BlurMakeSystem.SingleBlur)
                {
                    var batch = Main.spriteBatch;
                    batch.End();
                    BlurMakeSystem.KawaseBlur();
                    batch.Begin();
                }

                SDFRectangle.SampleVersion(BlurMakeSystem.BlurRenderTarget,
                    Bounds.Position * Main.UIScale, Bounds.Size * Main.UIScale, BorderRadius * Main.UIScale, Matrix.Identity);
            }

            base.Draw(gameTime, spriteBatch);
        }

        public override void HandleUpdate(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape))
                Enabled = false;

            base.HandleUpdate(gameTime);
        }
    }
}
