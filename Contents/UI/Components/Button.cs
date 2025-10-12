using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using FontStashSharp;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using TerraJS.Contents.Utils;
using TerraJS.Contents.UI;
using TerraJS.Contents.Extensions;

namespace TerraJS.Contents.UI.Components
{
    public class Button : Component
    {
        public Button(string texturePath, Vector2 relativePos = default, Action<Component> click = null, string text = "", Color? backgroundColor = null,
            Action<SpriteBatch, GameTime> drawing = null, string hoverTexturePath = "", string pressTexturePath = "", string fontID = "", int fontSize = 20,
            bool textHorizontalMiddle = true, bool textVerticalMiddle = true, bool drawShadow = true)
        {
            Texture = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad).Value;
            RelativePosition = relativePos == default ? new(0, 0) : relativePos;
            CanClick = true;
            BackgroundColor = backgroundColor ?? Color.Transparent;
            Text = text;
            Font = fontID == "" ? TerraJS.FontManager["Andy-Bold", fontSize] : TerraJS.FontManager[fontID, fontSize];
            TextHorizontalMiddle = textHorizontalMiddle;
            TextVerticalMiddle = textVerticalMiddle;
            DrawShadow = drawShadow;

            Drawing += drawing ?? ((spriteBatch, gameTime) => 
            {

            });

            OnClickEvent.AddListener("click", click != null ? click : _ => { });

            Hover = hoverTexturePath == "" ? Texture : ModContent.Request<Texture2D>(hoverTexturePath, AssetRequestMode.ImmediateLoad).Value;
            Press = pressTexturePath == "" ? Texture : ModContent.Request<Texture2D>(pressTexturePath, AssetRequestMode.ImmediateLoad).Value;
        }

        public int Timer;

        public event Action<SpriteBatch, GameTime> Drawing;

        public Texture2D Hover;

        public Texture2D Press;

        public bool TextHorizontalMiddle;

        public bool TextVerticalMiddle;

        public bool DrawShadow;

        public override int Height => Texture.Height;

        public override int Width => Texture.Width;

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * Alpha);

            var destination = RectangleUtils.FromPoint(Position.ToPoint() + (Size / 2).ToPoint(), Size.ToPoint());
            
            if (IsHovering)
            {
                if (UserInput.CurrentMouseState.LeftButton == ButtonState.Pressed)
                    spriteBatch.Draw(Press, destination, RectangleUtils.FromPoint(new(0, 0), Size.ToPoint()), Color.White * Alpha, Rotation, Size / 2, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(Hover, destination, RectangleUtils.FromPoint(new(0, 0), Size.ToPoint()), Color.White * Alpha, Rotation, Size / 2, SpriteEffects.None, 0);
            }
            else
                spriteBatch.Draw(Texture, destination, RectangleUtils.FromPoint(new(0, 0), Size.ToPoint()), Color.White * Alpha, Rotation, Size / 2, SpriteEffects.None, 0);

            if (!string.IsNullOrEmpty(Text))
            {
                var size = Font.MeasureString(Text);
                int x, y = 0;
                if (TextVerticalMiddle)
                    y = (int)(Position.Y + (Height - (int)Font.MeasureString(Text).Y) / 2) - 4;
                x = (int)Position.X;
                if (TextHorizontalMiddle)
                    x += Width / 2 - (int)size.X / 2;

                if (DrawShadow)
                    spriteBatch.DrawString(Font, Text, new(x + 2, y + 2), Color.Black * Alpha);

                spriteBatch.DrawString(Font, Text, new(x, y), BackgroundColor * Alpha);
            }
        }
    }
}
