using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerraJS.Contents.UI
{
    public interface ITJSUserInterface
    {
        public void PreDraw(SpriteBatch spriteBatch, GameTime gametime);

        public void Draw(SpriteBatch spriteBatch, GameTime gametime);

        public void PostDraw(SpriteBatch spriteBatch, GameTime gametime);

        public void Update(GameTime gameTime);

        public void Unload();
    }
}
