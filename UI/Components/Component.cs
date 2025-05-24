using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TerraJS.Extensions;
using TerraJS.UI.Components.Containers;
using FontStashSharp;

namespace TerraJS.UI.Components
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
            if (!Visible)
                return;
            if (BackgroundColor != default)
                spriteBatch.DrawRectangle(new((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor * _alpha);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = Rectangle.X + Rectangle.Width / 2 - _font.MeasureString(Text).X / 2;
                var y = Rectangle.Y + Rectangle.Height / 2 - _font.MeasureString(Text).Y / 2;

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), BackgroundColor * _alpha);
            }
        }

        public virtual void PostDraw(SpriteBatch spriteBatch, GameTime gameTime) { }

        internal void DrawBorder(SpriteBatch spriteBatch, Color? borderColor = null, bool drawOri = false)
        {
            Color[] data = new Color[_texture.Width * _texture.Height];
            _texture.GetData(data);
            data = (from c in data select c.A == 0 ? c : Color.White).ToArray();
            Texture2D t = new Texture2D(spriteBatch.GraphicsDevice, _texture.Width, _texture.Height);
            t.SetData(data);

            Color borderC = borderColor == null ? Color.White : (Color)borderColor;

            spriteBatch.Draw(t, Position - new Vector2(0, 2), borderC);
            spriteBatch.Draw(t, Position - new Vector2(2, 0), borderC);
            spriteBatch.Draw(t, Position + new Vector2(0, 2), borderC);
            spriteBatch.Draw(t, Position + new Vector2(2, 0), borderC);
            if (!drawOri) spriteBatch.Draw(t, Position, Color.White);
            else spriteBatch.Draw(_texture, Position, Color.White);
        }

        public virtual void LeftClick(object sender, int pressTime, Vector2 mouseStart)
        {
            var mouseRect = UserInput.GetMouseRectangle();

            if (mouseRect.Intersects(Rectangle))
            {
                Clicked = true;
                OnClick?.Invoke(this, new EventArgs());
            }
        }

        public virtual void RightClick(object sender, int pressTime, Vector2 mouseStart)
        {
            var mouseRect = UserInput.GetMouseRectangle();

            if (mouseRect.Intersects(Rectangle))
            {
                OnRightClick?.Invoke(this, new EventArgs());
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
            OnUpdate?.Invoke(this, new EventArgs());

            var mouseRect = UserInput.GetMouseRectangle();

            _isHovering = false;
            Clicked = false;

            if (mouseRect.Intersects(Rectangle))
            {
                _isHovering = true;
                OnHover?.Invoke(this, new EventArgs());
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

        public float _alpha = 1;

        public SpriteFontBase _font;

        public bool _isHovering;

        public Texture2D _texture;

        public event EventHandler OnClick;

        public event EventHandler OnHover;

        public event EventHandler OnUpdate;

        public event EventHandler OnRightClick;

        public float Scale;

        public Vector2 Size => new(Width, Height);

        public virtual int Height => _texture.Height;

        public virtual int Width => _texture.Width;

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
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public virtual float Alpha
        {
            get => _alpha;
            set => _alpha = value;
        }

        public string Text { get; set; }

        public virtual void Unload()
        {
            OnClick = null;
            OnHover = null;
            OnRightClick = null;
            OnUpdate = null;
            UserInput.LeftClick -= LeftClick;
            UserInput.RightClick -= RightClick;
            UserInput.KeepPressLeft -= KeepPressLeft;
            UserInput.KeepPressRight -= KeepPressRight;
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
