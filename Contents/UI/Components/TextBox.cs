using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using LibRimeDemo;
using LibRimeDemo.Data;
using TerraJS.Contents.UI.IME;
using Terraria;
namespace TerraJS.Contents.UI.Components
{
    public class TextBox : SizeContainer
    {
        public TextBox(int width, int height, int fontSize = 22, int leftOffset = 5, float charSpacing = 0, int keepPressDelay = 6, bool onlyNumber = false) : base(width, height)
        {
            Text = "";

            Font = TerraJS.FontManager["Andy-Bold", fontSize];

            BorderWidth.Set(1);

            Cursor = new(Color.White, Color.White)
            {
                TextBox = this
            };

            UserInput.CharPressed += CharPressed;
            UserInput.KeyJustPress += KeyJustPress;
            UserInput.KeyKeepPress += KeyKeepPress;

            LeftOffset = leftOffset;
            CharSpacing = charSpacing;
            OnlyNumber = onlyNumber;
            KeepPressDelay = keepPressDelay;

            BackgroundColor = ColorUtils.FromHex(0x393F8C) * 0.6f;

            TextColor = Color.White;
        }

        public static bool HasActiveInstance = false;

        public delegate void TextChangedHandler(TextBox sender, string text);

        public RimeApi IME => TerraJS.IME;

        private RimeSession _session;

        public TextChangedHandler OnTextChanged;

        public Cursor Cursor;

        public Color TextColor;

        public bool Active { get; set; }

        public int LeftOffset;

        public float CharSpacing;

        public bool OnlyNumber;

        public int KeepPressDelay;

        private void KeyJustPress(object sender, KeyEventArgs e)
        {
           if (Active)
            {
                if (IMEUtils.GetInput(_session) == "")
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            if (UserInput.Ctrl)
                            {
                                if (Text.Length > 0)
                                    Cursor.SelectFrom(0, Text.Length);
                            }
                            break;
                        case Keys.C:
                            if (UserInput.Ctrl)
                            {
                                if (Cursor.SelectBegin < Cursor.CursorIndex)
                                    Clipboard.SetClipboardText(Text.Substring(Cursor.SelectBegin, Cursor.CursorIndex - Cursor.SelectBegin));
                                if (Cursor.SelectBegin > Cursor.CursorIndex)
                                    Clipboard.SetClipboardText(Text.Substring(Cursor.CursorIndex, Cursor.SelectBegin - Cursor.CursorIndex));
                            }
                            break;
                        case Keys.V:
                            Cursor.DeleteSelection();

                            var clip = Clipboard.GetClipboardText();

                            if (OnlyNumber)
                            {
                                var regex = new Regex("^[0-9]");
                                clip = regex.Replace(clip, "");
                            }

                            if (UserInput.Ctrl)
                            {
                                Text = Text.Insert(Cursor.CursorIndex, clip);
                                Cursor.CursorIndex += clip.Length;
                                Cursor.SelectBegin = Cursor.CursorIndex;
                                OnTextChanged?.Invoke(null, Text);
                            }
                            break;
                        case Keys.X:
                            if (UserInput.Ctrl)
                            {
                                if (Cursor.SelectBegin < Cursor.CursorIndex)
                                    Clipboard.SetClipboardText(Text.Substring(Cursor.SelectBegin, Cursor.CursorIndex - Cursor.SelectBegin));
                                if (Cursor.SelectBegin > Cursor.CursorIndex)
                                    Clipboard.SetClipboardText(Text.Substring(Cursor.CursorIndex, Cursor.SelectBegin - Cursor.CursorIndex));
                                Cursor.DeleteSelection();
                                OnTextChanged?.Invoke(null, Text);
                            }
                            break;
                        case Keys.Left:
                            if (UserInput.Shift)
                                Cursor.CursorIndex--;
                            else if (UserInput.Ctrl)
                            {

                            }
                            else
                            {
                                Cursor.CursorIndex--;
                                Cursor.SelectBegin = Cursor.CursorIndex;
                            }
                            break;
                        case Keys.Right:
                            if (UserInput.Shift)
                                Cursor.CursorIndex++;
                            else if (UserInput.Ctrl)
                            {

                            }
                            else
                            {
                                Cursor.CursorIndex++;
                                Cursor.SelectBegin = Cursor.CursorIndex;
                            }
                            break;
                        case Keys.Back:
                            DeleteCharacter();
                            break;
                    }
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Back:
                            IMEUtils.BackSpace(_session);

                            break;

                        case Keys.Enter:
                            AppendString(IMEUtils.GetInput(_session).Replace("\'", ""));

                            IMEUtils.Escape(_session);

                            e.Cancel = true;

                            break;

                        case Keys.OemPlus:
                        case Keys.Add:
                            IMEUtils.NextPage(_session);

                            break;

                        case Keys.OemMinus:
                        case Keys.Subtract:
                            IMEUtils.PrevPage(_session);

                            break;

                        case Keys.Up:
                            IMEUtils.SelectPrev(_session);

                            break;

                        case Keys.Down:
                            IMEUtils.SelectNext(_session);

                            break;
                    }
                }

                if (e.KeyCode == Keys.Space)
                {
                    if (UserInput.Ctrl)
                    {
                        if (_session == null)
                        {
                            _session = IME.CreateSession();

                            IMEPanel.Instance.OpenIMEPanel(_session);

                            IMEPanel.Instance.MoveIMEPanel(this);
                        }
                        else
                        {
                            IME.DestroySession(_session);

                            IMEPanel.Instance.CloseIMEPanel();

                            _session = null;
                        }
                    }
                }
            }
        }

        private void KeyKeepPress(object sender, KeyEventArgs e)
        {
            if (Active)
            {
                if (IMEUtils.GetInput(_session) == "")
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Back:
                            Cursor.Timer[1]++;

                            if (Cursor.Timer[1] < KeepPressDelay)
                                return;

                            Cursor.Timer[1] = 0;

                            Cursor.ShouldBlink = false;

                            Cursor.Timer[0] = 0;

                            Cursor.Visible = true;

                            DeleteCharacter();
                            break;

                        case Keys.Left:
                            Cursor.Timer[2]++;

                            if (Cursor.Timer[2] < KeepPressDelay)
                                return;

                            Cursor.Timer[2] = 0;

                            Cursor.ShouldBlink = false;

                            Cursor.Timer[0] = 0;

                            Cursor.Visible = true;

                            if (UserInput.Shift)
                                Cursor.CursorIndex--;
                            else if (UserInput.Ctrl)
                            {

                            }
                            else
                            {
                                Cursor.CursorIndex--;
                                Cursor.SelectBegin = Cursor.CursorIndex;
                            }
                            break;

                        case Keys.Right:
                            Cursor.Timer[3]++;

                            if (Cursor.Timer[3] < KeepPressDelay)
                                return;

                            Cursor.Timer[3] = 0;

                            Cursor.ShouldBlink = false;

                            Cursor.Timer[0] = 0;

                            Cursor.Visible = true;

                            if (UserInput.Shift)
                                Cursor.CursorIndex++;
                            else if (UserInput.Ctrl)
                            {

                            }
                            else
                            {
                                Cursor.CursorIndex++;
                                Cursor.SelectBegin = Cursor.CursorIndex;
                            }
                            break;
                    }
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Back :
                            Cursor.Timer[4]++;

                            if (Cursor.Timer[4] < KeepPressDelay)
                                return;

                            Cursor.Timer[4] = 0;

                            IMEUtils.BackSpace(_session);

                            break;

                        case Keys.OemPlus:
                        case Keys.Add:
                            Cursor.Timer[5]++;

                            if (Cursor.Timer[5] < KeepPressDelay)
                                return;

                            Cursor.Timer[5] = 0;

                            IMEUtils.NextPage(_session);

                            break;

                        case Keys.OemMinus:
                        case Keys.Subtract:
                            Cursor.Timer[6]++;

                            if (Cursor.Timer[6] < KeepPressDelay)
                                return;

                            Cursor.Timer[6] = 0;

                            IMEUtils.PrevPage(_session);

                            break;

                        case Keys.Up:
                            Cursor.Timer[7]++;

                            if (Cursor.Timer[7] < KeepPressDelay)
                                return;

                            Cursor.Timer[7] = 0;

                            IMEUtils.SelectPrev(_session);

                            break;

                        case Keys.Down:
                            Cursor.Timer[8]++;

                            if (Cursor.Timer[8] < KeepPressDelay)
                                return;

                            Cursor.Timer[8] = 0;

                            IMEUtils.SelectNext(_session);

                            break;
                    }
                }
            }
        }

        private void CharPressed(object sender, CharacterEventArgs e)
        {
            if (Active && !UserInput.Ctrl)
            {
                if (_session == null || (e.Character == ' ' && IMEUtils.GetInput(_session) == ""))
                {
                    if (!e.Character.Equals('\r') && !e.Character.Equals('\n'))
                    {
                        if (OnlyNumber && (e.Character < 48 || e.Character > 57))
                            return;

                        AppendCharacter(e.Character);
                    }
                }
                else
                {
                    if (!e.Character.Equals('\r') && !e.Character.Equals('\n'))
                    {
                        if (OnlyNumber && (e.Character < 48 || e.Character > 57))
                            return;

                        if (e.Character >= 97 && e.Character <= 122)
                            _session.ProcessKey(e.Character);
                        else
                        {
                            var input = IMEUtils.GetInput(_session);

                            if (input != "")
                            {
                                var count = IMEUtils.GetCurrentPageCandidates(_session).Count;

                                if (count == 0)
                                {
                                    if (e.Character == ' ')
                                    {
                                        _session.ProcessKey(e.Character);

                                        var commit = IMEUtils.GetCommit(_session);

                                        AppendString(commit);
                                    }
                                }

                                if (count > 0)
                                {
                                    if (e.Character == ' ' || (e.Character >= 49 && e.Character <= count + 48))
                                    {
                                        _session.ProcessKey(e.Character);

                                        var commit = IMEUtils.GetCommit(_session);

                                        AppendString(commit);
                                    }
                                }
                            }
                            else
                            {
                                AppendCharacter(e.Character);
                            }
                        }
                    }
                }
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (Active)
                Cursor.DrawSelection(spriteBatch);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = Rectangle.X + LeftOffset;
                var y = Rectangle.Y + Rectangle.Height / 2 - Font.LineHeight / 2 + 2;

                spriteBatch.DrawBorderedStringWithSpace(Font, Text, new(x, y), TextColor * _alpha, Color.Black * _alpha, 2, CharSpacing);
            }

            if (Active)
                Cursor.Draw(spriteBatch);
        }

        public override void LeftClick(object sender, int pressTime, Vector2 mouseStart)
        {
            base.LeftClick(sender, pressTime, mouseStart);

            if (Clicked)
            {
                if (!Active) Active = true;
                else
                {
                    var mouse = UserInput.GetMousePos();

                    var index = GetCursorIndex(mouse);

                    Cursor.CursorIndex = index;
                    Cursor.SelectBegin = index;
                }
            }
            else
            {
                Active = false;
                Cursor.SelectBegin = Cursor.CursorIndex;
            }
        }

        public override void KeepMouseLeft(object sender, int pressTime, Vector2 mouseStart)
        {
            if (!Active) return;

            var mouse = UserInput.GetMousePos();

            var index = GetCursorIndex(mouse);

            var select = GetCursorIndex(mouseStart);

            Cursor.CursorIndex = index;
            Cursor.SelectBegin = select;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Cursor.Update(gameTime);

            BorderColor = BackgroundColor;

            Main.drawingPlayerChat = Active;

            if (Active)
            {
                HasActiveInstance = true;
                
                Main.LocalPlayer.mouseInterface = true;
            }

            if (!Active && _session != null)
            {
                IME.DestroySession(_session);

                IMEPanel.Instance.CloseIMEPanel();

                _session = null;
            }
        }

        public Vector2 TextSize(string text)
        {
            if (text.Length == 0)
                return Vector2.Zero;

            return Font.MeasureString(text, characterSpacing: CharSpacing);
        }

        public int GetCursorIndex(Vector2 mousePos)
        {
            for (var i = 1; i <= Text.Length; i++)
            {
                if (TextSize(Text[..i]).X + Position.X + LeftOffset > mousePos.X)
                    return i - 1;
            }
             
            return Text.Length;
        }

        public void AppendCharacter(char c)
        {
            Cursor.DeleteSelection();

            Text = Text.Insert(Cursor.CursorIndex, c.ToString());

            Cursor.CursorIndex++;

            Cursor.SelectBegin = Cursor.CursorIndex;

            OnTextChanged?.Invoke(null, Text);
        }

        public void AppendString(string str)
        {
            if (str == "")
                return;

            Cursor.DeleteSelection();

            Text = Text.Insert(Cursor.CursorIndex, str);

            Cursor.CursorIndex += str.Length;

            Cursor.SelectBegin = Cursor.CursorIndex;

            OnTextChanged?.Invoke(null, Text);
        }

        public void DeleteCharacter()
        {
            if (Cursor.DeleteSelection() && Cursor.CursorIndex > 0)
            {
                Text = Text.Remove(Cursor.CursorIndex - 1, 1);

                Cursor.SelectBegin--;

                Cursor.CursorIndex--;

                OnTextChanged?.Invoke(null, Text);
            }
        }

        public void ClearText()
        {
            Text = "";

            CursorTo(0);
        }

        public void CursorTo(int index)
        {
            Cursor.SelectBegin = index;

            Cursor.CursorIndex = index;
        }

        public override void Unload()
        {
            base.Unload();
            OnTextChanged = null;
        }
    }

    public class Cursor
    {
        public Cursor(Color cursorColor, Color selectionColor, int blinkTime = 40)
        {
            CursorColor = cursorColor;

            SelectionColor = selectionColor;

            BlinkTime = blinkTime;

            Visible = false;

            ShouldBlink = true;

            Timer = new(20);
        }

        public int BlinkTime;

        public Color CursorColor { get; set; }

        public Color SelectionColor { get; set; }

        public bool Visible;

        public bool ShouldBlink;

        public Timer Timer;

        public TextBox TextBox;

        public bool Active => TextBox.Active;

        public int CursorIndex
        {
            get => _cursorIndex;
            set => _cursorIndex = value.Clamp(0, TextBox.Text.Length);
        }

        private int _cursorIndex;

        public int SelectBegin
        {
            get => _selectBegin;
            set => _selectBegin = value.Clamp(0, TextBox.Text.Length);
        }

        private int _selectBegin;

        public void SelectFrom(int begin, int length)
        {
            SelectBegin = begin;
            CursorIndex = begin + length;
        }

        public bool DeleteSelection()
        {
            var res = CursorIndex == SelectBegin;
            if (SelectBegin < CursorIndex)
            {
                TextBox.Text = TextBox.Text.Remove(SelectBegin, CursorIndex - SelectBegin);
                CursorIndex = SelectBegin;
            }
            else
            {
                TextBox.Text = TextBox.Text.Remove(CursorIndex, SelectBegin - CursorIndex);
                SelectBegin = CursorIndex;
            }
            return res;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active) 
                return;

            if (ShouldBlink)
                Timer[0]++;

            if (Timer[0] >= BlinkTime)
            {
                Visible = !Visible;

                Timer[0] = 0;
            }

            ShouldBlink = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var cursorPos = TextBox.TextSize(TextBox.Text[..CursorIndex]);
            var selectBegin = TextBox.TextSize(TextBox.Text[..SelectBegin]).X;

            var x = (int)(cursorPos.X + TextBox.Position.X) + TextBox.LeftOffset;
            var y = (int)(TextBox.Height / 2 - TextBox.Font.FontSize / 2 + TextBox.Position.Y);

            if (Visible)
                spriteBatch.Draw(SpriteBatchExt.Pixel, new(x, y, 1, (int)TextBox.Font.FontSize), null, CursorColor, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        public void DrawSelection(SpriteBatch spriteBatch)
        {
            var cursorPos = TextBox.TextSize(TextBox.Text[..CursorIndex]);
            var selectBegin = TextBox.TextSize(TextBox.Text[..SelectBegin]).X;

            var x = (int)(cursorPos.X + TextBox.Position.X) + TextBox.LeftOffset;
            var y = (int)(TextBox.Height / 2 - TextBox.Font.FontSize / 2 + TextBox.Position.Y);

            if (selectBegin < cursorPos.X)
                spriteBatch.Draw(SpriteBatchExt.Pixel, new((int)selectBegin + (int)TextBox.Position.X + TextBox.LeftOffset, y, (int)cursorPos.X - (int)selectBegin, (int)TextBox.Font.FontSize), null, ColorUtils.FromHex(0x99C9EF) * 0.6f, 0, Vector2.Zero, SpriteEffects.None, 1);
            else if (selectBegin > cursorPos.X)
                spriteBatch.Draw(SpriteBatchExt.Pixel, new(x, y, (int)selectBegin - (int)cursorPos.X, (int)TextBox.Font.FontSize), null, ColorUtils.FromHex(0x99C9EF) * 0.6f, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}
