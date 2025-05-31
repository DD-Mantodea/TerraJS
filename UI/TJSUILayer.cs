using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using TerraJS.Extensions;
using TerraJS.UI.Components;
using Terraria.UI;
using TerraJS.UI.Components.Containers;

namespace TerraJS.UI
{
    public class TJSUILayer : GameInterfaceLayer
    {
        public TJSUILayer(string name, InterfaceScaleType scaleType) : base(name, scaleType) { }

        public SizeContainer ScreenContainer;

        public void Register(Component c) => ScreenContainer.RegisterChild(c);

        protected override bool DrawSelf()
        {
            var spriteBatch = Main.spriteBatch;

            spriteBatch.Rebegin(transformMatrix: Main.UIScaleMatrix);

            ScreenContainer.DrawSelf(spriteBatch, Main.gameTimeCache);

            spriteBatch.Change(transformMatrix: Matrix.Identity);

            return true;
        }

        public void Update(GameTime gameTime)
        {
            ScreenContainer.Update(gameTime);
        }
    }
}
