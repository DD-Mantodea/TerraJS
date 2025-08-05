using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TerraJS.Contents.Extensions
{
    public static class TextureExt
    {
        public static Texture2D Scale(this Texture2D texture, float scale)
        {
            int newWidth = (int)(texture.Width * scale);

            int newHeight = (int)(texture.Height * scale);

            return texture.ScaleTo(newWidth, newHeight);
        }

        public static Texture2D ScaleTo(this Texture2D texture, int width, int height)
        {
            var device = Main.spriteBatch.GraphicsDevice;

            var originalTargets = device.GetRenderTargets();

            var render = new RenderTarget2D(device, width, height);

            device.SetRenderTarget(render);

            device.Clear(Color.Transparent);

            var spriteBatch = new SpriteBatch(device);

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullNone
            );

            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);

            spriteBatch.End();

            device.SetRenderTargets(originalTargets);

            var scaled = new Texture2D(device, width, height);

            Color[] data = new Color[width * height];

            render.GetData(data);

            scaled.SetData(data);

            render.Dispose();

            return scaled;
        }
    }
}
