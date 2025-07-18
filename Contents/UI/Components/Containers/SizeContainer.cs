﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.DataStructures;
using TerraJS.Contents.Extensions;

namespace TerraJS.Contents.UI.Components.Containers
{
    public class SizeContainer : Container
    {
        public SizeContainer() { }

        public SizeContainer(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public SizeContainer(Vector2 size)
        {
            _width = (int)size.X;
            _height = (int)size.Y;
        }

        public Color BorderColor = Color.Black;

        public UIVec4 BorderWidth = new();

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * Alpha);

            spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, BorderWidth.X, Height), BorderColor);
            spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, BorderWidth.Y), BorderColor);
            spriteBatch.DrawRectangle(new(Width - BorderWidth.Z + (int)Position.X, (int)Position.Y, BorderWidth.Z, Height), BorderColor);
            spriteBatch.DrawRectangle(new((int)Position.X, Height - BorderWidth.W + (int)Position.Y, Width, BorderWidth.W), BorderColor);

            if (Scissor)
            {
                var state = spriteBatch.SaveState();

                spriteBatch.EnableScissor();

                spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle;

                DrawChildren(spriteBatch, gameTime);

                spriteBatch.LoadState(state);
            }
            else
                DrawChildren(spriteBatch, gameTime);

            //spriteBatch.DrawRectangle(RectangleUtils.FormVector2(Position, Size), Color.Green);
        }
    }
}
