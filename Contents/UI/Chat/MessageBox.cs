using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Assets.Managers;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using TerraJS.Hooks;
using Terraria;
using Terraria.GameInput;

namespace TerraJS.Contents.UI.Chat
{
    [RegisterUI("MessageBox")]
    public class MessageBox : SizeContainer
    {
        public static MessageBox Instance => UISystem.GetUIInstance<MessageBox>("MessageBox");

        public MessageBox()
        {
            _width = 600;

            _height = 400;

            InnerPos = new();
        }

        public Vector2 InnerPos;

        public override bool Visible => ChatBox.Instance?.Visible ?? false;

        public override void Update(GameTime gameTime)
        {
            RelativePosition = new(78, Main.screenHeight - 456);

            Visible = true;

            base.Update(gameTime);

            int totalHeight = Children.Sum(c => c.Height + 4);

            if (IsHovering)
            {
                var deltaWheel = UserInput.GetDeltaWheelValue();

                if (InnerPos.Y + Height < totalHeight)
                {
                    if (deltaWheel > 0)
                        InnerPos.Y += totalHeight / 20;
                }

                if (InnerPos.Y > 0)
                {
                    if (deltaWheel < 0)
                        InnerPos.Y -= totalHeight / 20;
                }

                PlayerHook.ShouldDisableScrollHotbar = true;
            }

            if (InnerPos.Y + Height > totalHeight)
                InnerPos.Y = totalHeight - Height;

            if (InnerPos.Y < 0)
                InnerPos.Y = 0;
        }

        public override void DrawSelf(SpriteBatch spriteBatch, GameTime gameTime)
        {
            PreDraw(spriteBatch, gameTime);

            Draw(spriteBatch, gameTime);

            PostDraw(spriteBatch, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var state = spriteBatch.SaveState();

            spriteBatch.EnableScissor();

            spriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Transform(Main.UIScaleMatrix);

            DrawChildren(spriteBatch, gameTime);

            spriteBatch.LoadState(state);
        }

        public void AppendMessage(string text, string player, Color color, bool local)
        {
            new ChatMessage(text, player, color, local).Join(this);

            if (Children.Count > 100)
                Children.RemoveAt(0);
        }

        public void ClearMessage()
        {
            Children.Clear();
        }

        public override void UpdateChildren(GameTime gameTime)
        {
            int totalHeight = Children.Sum(c => c.Height);

            var childPadding = 4;

            foreach (var child in Children)
            {
                child.RelativePosition.Y = Height - totalHeight + InnerPos.Y - (Children.Count - Children.IndexOf(child)) * childPadding;

                totalHeight -= child.Height;

                SetChildRelativePos(child);

                child.Update(gameTime);

                child.Position = child.RelativePosition - InnerPos + Position;
            }
        }
    }
}
