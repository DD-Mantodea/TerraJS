using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System.Reflection;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI;

namespace TerraJS.Contents.Extensions
{
    public static class SpriteBatchExt
    {
        private static Texture2D _pixel;
        public static Texture2D Pixel
        {
            get
            {
                if(_pixel == null)
                {
                    _pixel = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);

                    _pixel.SetData([ Color.White ]);
                }

                return _pixel;
            }
        }

        public static void DrawString(this SpriteBatch spriteBatch, TerraJSFont font, string text, Vector2 postion, Color color, float characterSpacing = 0)
        {
            font.DrawString(spriteBatch, text, postion, color, characterSpacing);
        }

        public static void DrawBorderedString(this SpriteBatch spriteBatch, TerraJSFont font, string text, Vector2 position, Color textColor, Color borderColor, int borderWidth)
        {
            spriteBatch.DrawString(font, text, position.Add(borderWidth, 0), borderColor);

            spriteBatch.DrawString(font, text, position.Add(0, borderWidth), borderColor);

            spriteBatch.DrawString(font, text, position.Sub(borderWidth, 0), borderColor);

            spriteBatch.DrawString(font, text, position.Sub(0, borderWidth), borderColor);

            spriteBatch.DrawString(font, text, position, textColor);
        }

        public static void DrawBorderedStringWithSpace(this SpriteBatch spriteBatch, TerraJSFont font, string text, Vector2 position, Color textColor, Color borderColor, int borderWidth, float charSpace)
        {
            spriteBatch.DrawString(font, text, position.Add(borderWidth, 0), borderColor, charSpace);

            spriteBatch.DrawString(font, text, position.Add(0, borderWidth), borderColor, charSpace);

            spriteBatch.DrawString(font, text, position.Sub(borderWidth, 0), borderColor, charSpace);

            spriteBatch.DrawString(font, text, position.Sub(0, borderWidth), borderColor, charSpace);

            spriteBatch.DrawString(font, text, position, textColor, charSpace);
        }

        public static void Rebegin(this SpriteBatch spriteBatch, SpriteSortMode? sortMode = null, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            var state = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(
                sortMode ?? state.SpriteSortMode, 
                blendState ?? state.BlendState, 
                samplerState ?? state.SamplerState, 
                depthStencilState ?? state.DepthStencilState, 
                rasterizerState ?? state.RasterizerState, 
                effect ?? state.Effect, 
                transformMatrix ?? state.Matrix
            );
        }

        public static void EnableScissor(this SpriteBatch spriteBatch)
        {
            var type = spriteBatch.GetType();
            var rState = (RasterizerState)type.GetField("rasterizerState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);
            spriteBatch.Change(rasterizerState: new() { ScissorTestEnable = true, CullMode = rState.CullMode });
        }

        public static void Change(this SpriteBatch spriteBatch, SpriteSortMode? sortMode = null, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            var type = spriteBatch.GetType();

            var sMode = sortMode ?? (SpriteSortMode)type.GetField("sortMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            var bState = blendState ?? (BlendState)type.GetField("blendState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            var sState = samplerState ?? (SamplerState)type.GetField("samplerState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            var dsState = depthStencilState ?? (DepthStencilState)type.GetField("depthStencilState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            var rState = rasterizerState ?? (RasterizerState)type.GetField("rasterizerState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            var efct = effect ?? (Effect)type.GetField("spriteEffect", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            var matrix = transformMatrix ?? (Matrix)type.GetField("transformMatrix", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            spriteBatch.Rebegin(sMode, bState, sState, dsState, rState, efct, matrix);
        }

        public static SpriteBatchState SaveState(this SpriteBatch spriteBatch)
        {
            var state = new SpriteBatchState();

            var type = spriteBatch.GetType();

            state.SpriteSortMode = (SpriteSortMode)type.GetField("sortMode", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            state.BlendState = (BlendState)type.GetField("blendState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            state.SamplerState = (SamplerState)type.GetField("samplerState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            state.DepthStencilState = (DepthStencilState)type.GetField("depthStencilState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            state.RasterizerState = (RasterizerState)type.GetField("rasterizerState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            state.Effect = (Effect)type.GetField("spriteEffect", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            state.Matrix = (Matrix)type.GetField("transformMatrix", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(spriteBatch);

            return state;
        }

        public static void LoadState(this SpriteBatch spriteBatch, SpriteBatchState state)
        {
            spriteBatch.Rebegin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);
        }

        public static void DrawLine(this SpriteBatch batch, Line line, Color color)
        {
            float radian = line.ToVector2().GetRadian();
            if (Pixel is not null)
                batch.Draw(Pixel, line.Start, null, color, radian, Vector2.Zero, new Vector2(Vector2.Distance(line.Start, line.End), 1f), SpriteEffects.None, 0);
        }

        public static void DrawLine(this SpriteBatch batch, Vector2 start, Vector2 end, Color color)
        {
            batch.DrawLine(new Line(start, end), color);
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle rect, Color color)
        {
            batch.Draw(Pixel, rect, color);
        }

        public static void DrawRectangle(this SpriteBatch batch, Rectangle rect, Color color, float rotation = 0f, Vector2 origin = default, float scale = 1, SpriteEffects effects = SpriteEffects.None, float layerDepth = 1)
        {
            batch.Draw(Pixel, rect, null, color, rotation, origin, effects, layerDepth);
        }

        public static void DrawSnippets(this SpriteBatch spriteBatch, TerraJSFont font, List<TextSnippet> snippets, Vector2 position)
        {
            foreach (var snippet in snippets)
            {
                snippet.Draw(spriteBatch, font, ref position);
            }
        }
    }

    public struct Line
    {
        public Vector2 Start;
        public Vector2 End;
        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        public Vector2 ToVector2()
        {
            return End - Start;
        }
    }

    public class SpriteBatchState
    {
        public SpriteBatchState() { }

        public SpriteSortMode SpriteSortMode { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }

        public Effect Effect { get; set; }
        public Matrix Matrix { get; set; }

        public void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSortMode, Effect effect = null, Matrix? matrix = null)
        {
            spriteBatch.Begin(spriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, effect, matrix ?? Matrix);
        }
    }
}
