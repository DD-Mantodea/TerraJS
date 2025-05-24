using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using TerraJS.Extensions;
using TerraJS.Utils;
using Terraria;

namespace TerraJS.API.Quests.QuestGUI
{
    public class QuestViewer : UIElementGroup
    {
        public Matrix View;

        public Matrix Projection;

        public Matrix Transform => View * Projection;

        public float ZoomScale;

        public Vector2 CameraPos;

        public UIElementGroup Container;

        protected override void OnInitialize()
        {
            CameraPos = new(0, 0);

            BackgroundColor = Color.Green;

            MouseWheel += (e, ui) =>
            {
                var deltaWheel = e.ScrollDelta;

                if (deltaWheel > 0)
                    ZoomCamera(1.25f);
                if (ZoomScale > 5) ZoomScale = 4;

                if (deltaWheel < 0)
                    ZoomCamera(0.8f);
                if (ZoomScale < 0.2) ZoomScale = 0.25f;
            };

            RightMouseClick += (e, ui) =>
            {
                if (QuestPanel.Instance.EditingMode)
                    AddNode(e.MousePosition - new Vector2(32, 32));
            };

            LayoutType = LayoutType.Custom;

            ZoomScale = 1f;

            Projection = Matrix.Identity;

            View = Matrix.Identity;
        }

        public void MoveCamera(Vector2 vec) => CameraPos += vec;

        public void ZoomCamera(float scale)
        {
            ZoomScale *= scale;

            ZoomScale = (float)Math.Round(ZoomScale, 4);
        }

        public override void HandleUpdate(GameTime gameTime)
        {
            base.HandleUpdate(gameTime);

            View = Matrix.CreateTranslation(new(-Bounds.Center, 0)) *
                Matrix.CreateScale(ZoomScale, ZoomScale, 1) *
                Matrix.CreateTranslation(new(Bounds.Center, 0)) *
                Matrix.CreateTranslation(new(CameraPos * ZoomScale, 0));
        }

        public QuestNode AddNode(Vector2 pos)
        {
            var node = new QuestNode().Join(this);

            node.Initialize();

            node.SetLeft(pos.X - OuterBounds.Left);

            node.SetTop(pos.Y - OuterBounds.Top);

            return node;
        }

        public Vector2 ScreenToWorld(Vector2 screenPos) => Vector2.Transform(screenPos, Matrix.Invert(Transform));

        public Vector2 WorldToScreen(Vector2 worldPos) => Vector2.Transform(worldPos, Transform);

        public override void DrawChildren(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Bounds innerBounds = InnerBounds;

            spriteBatch.End();

            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

            Rectangle scissorRectangle2 = Rectangle.Intersect(GetClippingRectangle(spriteBatch), scissorRectangle);

            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;

            RenderStates renderStates = RenderStates.BackupStates(Main.graphics.GraphicsDevice, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, SilkyUI.RasterizerStateForOverflowHidden, null, Transform * SilkyUI.TransformMatrix);

            foreach (UIView item in ElementsSortedByZIndex.Where((UIView el) => el.GetOuterBounds().Intersects(innerBounds)))
                item.HandleDraw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

            renderStates.Begin(spriteBatch, SpriteSortMode.Deferred);
        }
    }
}
