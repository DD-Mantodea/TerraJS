using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FontStashSharp;
using Terraria;
using TerraJS.Contents.UI;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.DataStructures;
using TerraJS.Contents.Attributes;

namespace TerraJS.Contents.UI.Components
{
    public class Component : ITJSUserInterface
    {
        public Component()
        {
            UserInput.LeftClick += LeftClick;

            UserInput.RightClick += RightClick;

            UserInput.KeepPressLeft += KeepPressLeft;

            UserInput.KeepPressRight += KeepPressRight;
        }

        public virtual void PreDraw(SpriteBatch spriteBatch, GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * Alpha);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (int)Position.X + Width / 2 - Font.MeasureString(Text).X / 2;
                var y = (int)Position.Y + Height / 2 - Font.MeasureString(Text).Y / 2;

                spriteBatch.DrawString(Font, Text, new Vector2(x, y), BackgroundColor * Alpha);
            }
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, GameTime gameTime) { }

        public virtual void DrawSelf(SpriteBatch spriteBatch, GameTime gameTime)
        {
            PreDraw(spriteBatch, gameTime);

            if (Visible)
                Draw(spriteBatch, gameTime);

            PostDraw(spriteBatch, gameTime);
        }

        public virtual void LeftClick(object sender, int pressTime, Vector2 mouseStart)
        {
            var mouseRect = UserInput.GetMouseRectangle();

            if (mouseRect.Intersects(Rectangle))
            {
                Clicked = true;
                OnClickEvent?.Invoke(this);
            }
        }

        public virtual void RightClick(object sender, int pressTime, Vector2 mouseStart)
        {
            var mouseRect = UserInput.GetMouseRectangle();

            if (mouseRect.Intersects(Rectangle))
            {
                OnRightClickEvent?.Invoke(this);
            }
        }

        public virtual void KeepPressLeft(object sender, int pressTime, Vector2 mouseStart)
        {

        }

        public virtual void KeepPressRight(object sender, int pressTime, Vector2 mouseStart)
        {

        }

        public virtual void Update(GameTime gameTime)
        {
            OnUpdateEvent?.Invoke(this);

            UpdateMouse(gameTime);

            DrawOffset = new(0, 0);

            Position = RelativePosition + (Parent == null ? new(0, 0) : Parent.Position);
        }

        public virtual void UpdateMouse(GameTime gameTime)
        {
            var mouseRect = UserInput.GetMouseRectangle();

            IsHovering = false;

            Clicked = false;

            if (mouseRect.Intersects(Rectangle))
            {
                IsHovering = true;
                OnHoverEvent?.Invoke(this);
            }
        }

        internal int _width = 0;

        internal int _height = 0;

        internal float _alpha = 1;

        public Matrix View => Main.UIScaleMatrix;

        public Matrix SelfMatrix = Matrix.Identity;

        public Matrix Transform => View * SelfMatrix;

        public virtual float Alpha { get => _alpha; set => _alpha = value; }

        public SpriteFontBase Font;

        public bool IsHovering;

        public Texture2D Texture;

        public UIEvent OnClickEvent = new();

        public UIEvent OnRightClickEvent = new();

        public UIEvent OnHoverEvent = new();

        public UIEvent OnUpdateEvent = new();

        public float Scale;

        public Vector2 Size => new(Width, Height);

        public virtual int Height => _height;

        public virtual int Width => _width;

        public bool Clicked { get; internal set; }

        public Color BackgroundColor;

        public Vector2 Position = new(0, 0);

        public Vector2 RelativePosition = new(0, 0);

        public float Rotation;

        public bool shouldCollect = false;

        public Vector2 DrawOffset = new(0, 0);

        public float DrawScale = 1f;

        public Container Parent;

        public bool CanClick = true;

        public string ID = "";

        public bool HorizontalMiddle;
        public bool VerticalMiddle;

        public Anchor Anchor = Anchor.None;

        public virtual bool Visible { get; set; } = true;

        public virtual Rectangle Rectangle
        {
            get
            {
                var screenPos = ScreenToWorld(Position);

                var realSize = ScreenToWorld(Size);

                return new Rectangle((int)screenPos.X, (int)screenPos.Y, (int)realSize.X, (int)realSize.Y);
            }
        }

        public virtual Vector2 ScreenToWorld(Vector2 vec) => Vector2.Transform(vec, Transform);

        public virtual Vector2 WorldToScreen(Vector2 vec) => Vector2.Transform(vec, Matrix.Invert(Transform));

        public string Text { get; set; }

        public virtual void Unload()
        {
            OnClickEvent = null;
            OnHoverEvent = null;
            OnRightClickEvent = null;
            OnUpdateEvent = null;
            UserInput.LeftClick -= LeftClick;
            UserInput.RightClick -= RightClick;
            UserInput.KeepPressLeft -= KeepPressLeft;
            UserInput.KeepPressRight -= KeepPressRight;
        }

        public void SetSize(int width, int height)
        {
            _width = width;

            _height = height;
        }
    }
}

public enum Anchor
{
    Bottom,
    Left,
    Right,
    Top,
    None
}
