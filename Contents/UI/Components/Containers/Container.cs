using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Utils;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components;

namespace TerraJS.Contents.UI.Components.Containers
{
    public abstract class Container : Component
    {
        public List<Component> Children = new List<Component>();

        public bool Scissor = false;

        public virtual void RegisterChild(Component component)
        {
            if (SelectChildById(component.ID) != null && component.ID != "")
                return;
            component.Parent = this;
            Children.Add(component);
        }

        public virtual void SetChildRelativePos(Component child)
        {
            if (child.HorizontalMiddle)
                child.RelativePosition.X = (Width - child.Width) / 2;
            if (child.VerticalMiddle)
                child.RelativePosition.Y = (Height - child.Height) / 2;

            switch (child.Anchor)
            {
                case Anchor.Left:
                    child.RelativePosition.X = 0;
                    break;
                case Anchor.Right:
                    child.RelativePosition.X = Width - child.Width;
                    break;
                case Anchor.Top:
                    child.RelativePosition.Y = 0;
                    break;
                case Anchor.Bottom:
                    child.RelativePosition.Y = Height - child.Height;
                    break;
                case Anchor.None:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateChildren(gameTime);
        }

        public virtual void UpdateChildren(GameTime gameTime)
        {
            foreach (var child in Children)
            {
                if(child.Visible)
                    SetChildRelativePos(child);

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
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (Scissor)
            {
                var state = spriteBatch.SaveState();

                spriteBatch.EnableScissor();

                spriteBatch.GraphicsDevice.ScissorRectangle = RectangleUtils.FormVector2(Position, Size);

                DrawChildren(spriteBatch, gameTime);

                spriteBatch.End();
            }
            else
                DrawChildren(spriteBatch, gameTime);
        }

        public virtual void DrawChildren(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var component in Children)
                component.DrawSelf(spriteBatch, gameTime);
        }

        public virtual void RegisterChildAt(int index, Component component, bool behind = false)
        {
            if (SelectChildById(component.ID) != null)
                return;
            if (behind)
                Children.Insert(Children.Count - index, component);
            else
                Children.Insert(index, component);

            SetChildRelativePos(component);
        }

        public bool ContainsChild(Predicate<Component> match)
        {
            return Children.Exists(match);
        }

        public Component SelectChild(Func<Component, bool> match)
        {
            return Children.FirstOrDefault(match, null);
        }

        public T SelectChild<T>(Func<Component, bool> match) where T : Component
        {
            if (Children.FirstOrDefault(match) is not T) return null;
            return Children.FirstOrDefault(match, null) as T;
        }

        public List<Component> SelectChildren(Func<Component, bool> match)
        {
            return Children.Where(match).ToList();
        }

        public T SelectChildById<T>(string id) where T : Component
        {
            if (id == "") return null;
            if (Children.FirstOrDefault(c => c.ID == id, null) is not T) return null;
            else return Children.FirstOrDefault(c => c.ID == id, null) as T;
        }

        public Component SelectChildById(string id)
        {
            if (id == "") return null;
            return Children.FirstOrDefault(c => c.ID == id, null);
        }

        public void RemoveAllChild()
        {
            Children.Clear();
        }

        public override float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                foreach (var components in Children)
                    components.Alpha = value;
            }
        }

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                foreach (var components in Children)
                    components.Visible = value;
            }
        }

        public override void Unload()
        {
            base.Unload();
            foreach (var component in Children)
                component.Unload();
        }
    }
}
