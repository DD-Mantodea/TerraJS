using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components.Containers;
using Terraria;
using Terraria.UI;

namespace TerraJS.Contents.UI
{
    public class UILayer : GameInterfaceLayer
    {
        public UILayer(string name, InterfaceScaleType scaleType) : base(name, scaleType) { }

        public SizeContainer ScreenContainer = new(Main.screenWidth, Main.screenHeight);

        public void Register(Container c)
        {
            var index = ScreenContainer.Children.FindIndex(0, ScreenContainer.Children.Count, child => (child as Container).Priority > c.Priority);

            ScreenContainer.RegisterChildAt(index == -1 ? 0 : index, c);
        }

        public List<Container> UIList => [..ScreenContainer.Children.Select(c => c as Container)];

        public bool TryGetUIByID(string ID, out Container c)
        {
            c = UIList.FirstOrDefault(c => c.ID == ID, null);

            return c != null;
        }

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
