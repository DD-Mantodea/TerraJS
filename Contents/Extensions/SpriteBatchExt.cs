﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System.Reflection;
using System.Text.RegularExpressions;
using Terraria.UI.Chat;
using FontStashSharp;

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

        public static Vector2 DrawColorCodedString(this SpriteBatch spriteBatch, SpriteFontBase font, List<TextSnippet> snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors = false)
        {
            if (baseColor == Color.Transparent)
            {
                hoveredSnippet = -1;
                return Vector2.Zero;
            }

            var num1 = -1;
            var vec = new Vector2(Main.mouseX, Main.mouseY);
            var vector2_1 = position;
            var vector2_2 = vector2_1;
            var x = font.MeasureString(" ").X;
            var color = baseColor;
            var num2 = 0.0f;
            for (var index1 = 0; index1 < snippets.Count; ++index1)
            {
                var snippet = snippets[index1];
                snippet.Update();
                if (!ignoreColors)
                    color = snippet.GetVisibleColor();
                var scale = snippet.Scale;
                if (snippet.UniqueDraw(false, out var size, spriteBatch, vector2_1, color, baseScale.X * scale))
                {
                    if (vec.Between(vector2_1, vector2_1 + size))
                        num1 = index1;
                    vector2_1.X += size.X;
                    vector2_2.X = Math.Max(vector2_2.X, vector2_1.X);
                }
                else
                {
                    snippet.Text.Split('\n');
                    string[] strArray1 = Regex.Split(snippet.Text, "(\n)");
                    bool flag = true;
                    foreach (string input in strArray1)
                    {
                        Regex.Split(input, "( )");
                        string[] strArray2 = input.Split(' ');
                        if (input == "\n")
                        {
                            vector2_1.Y += font.LineHeight * num2 * baseScale.Y;
                            vector2_1.X = position.X;
                            vector2_2.Y = Math.Max(vector2_2.Y, vector2_1.Y);
                            num2 = 0.0f;
                            flag = false;
                        }
                        else
                        {
                            for (int index2 = 0; index2 < strArray2.Length; ++index2)
                            {
                                if (index2 != 0)
                                    vector2_1.X += x * baseScale.X * scale;
                                if ((double)maxWidth > 0.0)
                                {
                                    float num3 = font.MeasureString(strArray2[index2]).X * baseScale.X * scale;
                                    if (vector2_1.X - (double)position.X + (double)num3 > (double)maxWidth)
                                    {
                                        vector2_1.X = position.X;
                                        vector2_1.Y += font.LineHeight * num2 * baseScale.Y;
                                        vector2_2.Y = Math.Max(vector2_2.Y, vector2_1.Y);
                                        num2 = 0.0f;
                                    }
                                }

                                if ((double)num2 < (double)scale)
                                    num2 = scale;
                                spriteBatch.DrawString(font, strArray2[index2], vector2_1, color, rotation, origin,
                                    baseScale * snippet.Scale * scale);
                                Vector2 vector2_3 = font.MeasureString(strArray2[index2]);
                                if (vec.Between(vector2_1, vector2_1 + vector2_3))
                                    num1 = index1;
                                vector2_1.X += vector2_3.X * baseScale.X * scale;
                                vector2_2.X = Math.Max(vector2_2.X, vector2_1.X);
                            }

                            if (strArray1.Length > 1 & flag)
                            {
                                vector2_1.Y += font.LineHeight * num2 * baseScale.Y;
                                vector2_1.X = position.X;
                                vector2_2.Y = Math.Max(vector2_2.Y, vector2_1.Y);
                                num2 = 0.0f;
                            }

                            flag = true;
                        }
                    }
                }
            }

            hoveredSnippet = num1;
            return vector2_2;
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
