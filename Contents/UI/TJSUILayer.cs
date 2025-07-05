using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;

namespace TerraJS.Contents.UI
{
    public class TJSUILayer : GameInterfaceLayer
    {
        public TJSUILayer(string name, InterfaceScaleType scaleType) : base(name, scaleType) { }

        public SizeContainer ScreenContainer;

        public void Register(Component c) => ScreenContainer.RegisterChild(c);

        protected override bool DrawSelf()
        {
            var spriteBatch = Main.spriteBatch;

            var state = spriteBatch.SaveState();

            spriteBatch.Change(transformMatrix: Main.UIScaleMatrix);

            ScreenContainer.DrawSelf(spriteBatch, Main.gameTimeCache);

            spriteBatch.Change(transformMatrix: state.Matrix);

            return true;
        }

        public void Update(GameTime gameTime)
        {
            ScreenContainer.Update(gameTime);
        }
    }
}
