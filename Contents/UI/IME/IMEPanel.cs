using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;
using LibRimeDemo.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using TerraJS.Hooks;
using Terraria;

namespace TerraJS.Contents.UI.IME
{
    [RegisterUI("IMEPanel")]
    public class IMEPanel : SizeContainer
    {
        public static IMEPanel Instance => UISystem.GetUIInstance<IMEPanel>("IMEPanel");

        private RimeSession _session = null;

        public IMEPanel()
        {
            BackgroundColor = ColorUtils.FromHex(0x393F8C) * 0.6f;

            BorderColor = ColorUtils.FromHex(0x393F8C);

            BorderWidth.Set(2);

            _height = 60;

            _width = 300;

            Font = TerraJS.FontManager["YaHei", 22];

            Visible = false;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IMEUtils.GetInput(_session) != "")
            {
                base.Draw(spriteBatch, gameTime);

                spriteBatch.DrawBorderedString(Font, IMEUtils.GetInput(_session), Position.Add(4, 4), Color.White * 0.8f, Color.Black * 0.8f, 2);

                spriteBatch.Draw(SpriteBatchExt.Pixel, new Rectangle((int)Position.X + 4, (int)Position.Y + 25, Width - 8, 1), Color.White * 0.6f);

                var candidates = IMEUtils.GetCurrentPageCandidates(_session);

                var offset = 0;

                var index = IMEUtils.GetSelectCandidateIndex(_session);

                for (var i = 0; i < candidates.Count; i++)
                {
                    var candidate = candidates[i];

                    var text = $"{i + 1}. {candidate}";

                    if (index == i)
                    {
                        spriteBatch.Draw(SpriteBatchExt.Pixel, RectangleUtils.FromVector2(Position.Add(10 + offset, 35).Sub(2, 4), Font.MeasureString(text).Add(4, 8)), ColorUtils.FromHex(0x5D7BA1) * 0.6f);

                        spriteBatch.DrawBorderedString(Font, text, Position.Add(10 + offset, 35), ColorUtils.FromHex(0xFFD90D) * 0.9f, Color.Black * 0.9f, 2);
                    }
                    else
                        spriteBatch.DrawBorderedString(Font, text, Position.Add(10 + offset, 35), Color.White * 0.9f, Color.Black * 0.9f, 2);

                    offset += (int)Font.MeasureString(text).X + 10;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var candidates = IMEUtils.GetCurrentPageCandidates(_session);

            var offset = 0;

            for (var i = 0; i < candidates.Count; i++)
            {
                var text = $"{i + 1}. {candidates[i]}";

                offset += (int)Font.MeasureString(text).X + 10;
            }

            _width = Math.Max(300, 10 + offset);

            if (IsHovering)
            {
                var deltaWheel = UserInput.GetDeltaWheelValue();

                if (deltaWheel < 0)
                    IMEUtils.SelectNext(_session);

                if (deltaWheel > 0)
                    IMEUtils.SelectPrev(_session);

                PlayerHook.ShouldDisableScrollHotbar = true;
            }
        }

        public void OpenIMEPanel(RimeSession session)
        {
            _session = session;

            Visible = true;
        }

        public void CloseIMEPanel()
        {
            _session = null;

            Visible = false;
        }

        public void MoveIMEPanel(TextBox textBox)
        {
            RelativePosition.X = textBox.Position.X;

            RelativePosition.Y = textBox.Position.Y + textBox.Height + Height + 10;

            if (RelativePosition.Y > Main.screenHeight)
                RelativePosition.Y = textBox.Position.Y - Height - 10;
        }
    }
}
