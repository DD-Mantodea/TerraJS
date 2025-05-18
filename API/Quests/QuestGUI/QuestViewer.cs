using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
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

        protected override void OnInitialize()
        {
            CameraPos = new(0, 0);

            LayoutType = LayoutType.Custom;

            ZoomScale = 1f;

            OverflowHidden = true;

            Projection = Matrix.Identity;

            View = Matrix.Identity;

            MouseWheel += (e, ui) =>
            {
                var deltaWheel = e.ScrollDelta;

                if (deltaWheel > 0)
                    ZoomCamera(1.25f);
                if (ZoomScale > 5) ZoomScale = 5;

                if (deltaWheel < 0)
                    ZoomCamera(0.8f);
                if (ZoomScale < 0.2) ZoomScale = 0.2f;
            };

            RightMouseClick += (e, ui) =>
            {
                if (QuestPanel.Instance.EditingMode && GetElementAt(e.MousePosition) == this)
                    AddNode(e.MousePosition);
            };
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

            var Size = Bounds.Size.ToVec2();

            View = Matrix.CreateTranslation(new(-Size / 2, 0)) *
                Matrix.CreateScale(ZoomScale, ZoomScale, 1) *
                Matrix.CreateTranslation(new(Size / 2, 0)) *
                Matrix.CreateTranslation(new(CameraPos * ZoomScale, 0));

            SilkyUI.TransformMatrix = Transform;
        }

        public QuestNode AddNode(Vector2 pos)
        {
            var node = new QuestNode().Join(this);

            node.Initialize();

            node.SetSize(10, 10);

            node.SetLeft(pos.X - OuterBounds.Left);

            node.SetTop(pos.Y - OuterBounds.Top);

            return node;
        }
    }
}
