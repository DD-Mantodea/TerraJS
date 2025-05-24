using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TerraJS.Extensions;
using Terraria;

namespace TerraJS.UI.Components.Containers
{
    public class ZoomableContainer : SizeContainer
    {
        public float ZoomScale;

        public Vector2 CameraPosition;

        public ZoomableContainer(int width, int height) 
        { 
            _width = width;
            _height = height;
        }

        public ZoomableContainer(Vector2 size)
        {
            _width = (int)size.X;
            _height = (int)size.Y;
        }

        public void Initialize()
        {
            CameraPosition = new(0, 0);
            ZoomScale = 1f;
            Projection = Matrix.Identity;
            View = Matrix.Identity;
        }

        public void MoveCamera(Vector2 vec)
        {
            CameraPosition += vec;
        }

        public void ZoomCamera(float scale)
        {
            ZoomScale *= scale;
            ZoomScale = (float)Math.Round(ZoomScale, 4);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible) return;
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * _alpha);

            spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, BorderWidth.X, _height), BorderColor);
            spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, _width, BorderWidth.Y), BorderColor);
            spriteBatch.DrawRectangle(new(_width - BorderWidth.Z + (int)Position.X, (int)Position.Y, BorderWidth.Z, Height), BorderColor);
            spriteBatch.DrawRectangle(new((int)Position.X, _height - BorderWidth.W + (int)Position.Y, _width, BorderWidth.W), BorderColor);

            spriteBatch.Rebegin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone, 
                transformMatrix: Transform);
            spriteBatch.EnableScissor();
            spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle;

            foreach (var component in Children)
                component.Draw(spriteBatch, gameTime);

            spriteBatch.Rebegin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
            spriteBatch.GraphicsDevice.ScissorRectangle = RectangleExt.FormPoint(Point.Zero, Main.ScreenSize);
        }

        public override void Update(GameTime gameTime)
        {
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                MoveCamera(new(-2, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                MoveCamera(new(2, 0));
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                MoveCamera(new(0, -2));
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                MoveCamera(new(0, 2));
            */

            var deltaWheel = UserInput.GetDeltaWheelValue();

            if (_isHovering)
            {
                if (deltaWheel > 0)
                    ZoomCamera(1.25f);
                if (ZoomScale > 5) ZoomScale = 5;
                if (deltaWheel < 0)
                    ZoomCamera(0.8f);
                if (ZoomScale < 0.2) ZoomScale = 0.2f;
            }

            View = Matrix.CreateTranslation(new(-Position - Size / 2, 0)) *
                Matrix.CreateScale(ZoomScale, ZoomScale, 1) *
                Matrix.CreateTranslation(new(Position + Size / 2, 0)) *
                Matrix.CreateTranslation(new(CameraPosition * ZoomScale, 0));

            TerraJS.Instance.CurrentProjection = Projection;
            TerraJS.Instance.CurrentView = View;

            foreach (var child in Children)
            {
                child.Position = child.RelativePosition + Position;
                child.Update(gameTime);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].shouldCollect)
                {
                    Children.RemoveAt(i);
                    i--;
                }
            }

            var mouseRect = UserInput.GetMouseRectangle();

            _isHovering = false;

            if (mouseRect.Intersects(Rectangle))
            {
                _isHovering = true;
            }

            if (HorizontalMiddle) RelativePosition.X = (Parent.Width - Width) / 2;
            if (VerticalMiddle) RelativePosition.Y = (Parent.Height - Height) / 2;

            switch (Anchor)
            {
                case Anchor.Left:
                    RelativePosition.X = 0;
                    break;
                case Anchor.Right:
                    RelativePosition.X = Parent.Width - Width;
                    break;
                case Anchor.Top:
                    RelativePosition.Y = 0;
                    break;
                case Anchor.Bottom:
                    RelativePosition.Y = Parent.Height - Height;
                    break;
                case Anchor.None:
                    break;
            }

            DrawOffset = new(0, 0);
        }
    }
}
