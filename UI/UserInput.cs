using TerraJS.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;

namespace TerraJS.UI
{
    public class UserInput
    {
        public static KeyboardState CurrentKeyState;

        public static KeyboardState PreviousKeyState;

        public static MouseState CurrentMouseState;

        public static MouseState PreviousMouseState;

        public delegate void MouseEventHandler(object sender, int pressTime, Vector2 mouseStart);

        public delegate void KeyEventHandler(object sender, KeyEventArgs e);

        public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);

        public static event MouseEventHandler LeftClick;

        public static event MouseEventHandler KeepPressLeft;

        public static event MouseEventHandler RightClick;

        public static event MouseEventHandler KeepPressRight;

        public static event KeyEventHandler KeyPressed;

        public static event KeyEventHandler KeyKeepPress;

        public static event KeyEventHandler KeyJustPress;

        public static event CharEnteredHandler CharPressed;

        public static readonly List<char> SpecialCharacters = new() { '\a', '\b', '\n', '\r', '\f', '\t', '\v' };

        private static Dictionary<Keys, int> _pressTime;

        private static int _leftPressTime;

        private static int _rightPressTime;

        private static Vector2 _leftPressStart;

        private static Vector2 _rightPressStart;

        public static void Initialize()
        {
            TextInputEXT.TextInput += TextInput;

            _pressTime = new();
            _leftPressTime = 0;
            _rightPressTime = 0;

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                _pressTime[key] = 0;
        }

        private static void TextInput(char c)
        {
            if (!SpecialCharacters.Contains(c))
                CharPressed?.Invoke(null, new CharacterEventArgs(c));
        }

        public static void GetState()
        {
            PreviousKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public static void Update()
        {
            GetState();

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (IsJustPress(key))
                {
                    KeyJustPress?.Invoke(null, new KeyEventArgs(key));
                }
                if (IsKeyDown(key))
                {
                    _pressTime[key]++;
                    if (_pressTime[key] > 20)
                        KeyKeepPress?.Invoke(null, new KeyEventArgs(key));
                }
                else
                {
                    if (_pressTime[key] <= 20 && _pressTime[key] != 0)
                        KeyPressed?.Invoke(null, new KeyEventArgs(key));
                    _pressTime[key] = 0;
                }
            }

            if (MouseLeft)
            {
                if (_leftPressTime == 0)
                    _leftPressStart = CurrentMouseState.Position();
                _leftPressTime++;
                if (_leftPressTime > 20)
                    KeepPressLeft?.Invoke(null, _leftPressTime, _leftPressStart);
            }
            else
            {
                if (_leftPressTime <= 20 && _leftPressTime != 0)
                    LeftClick?.Invoke(null, _leftPressTime, _leftPressStart);

                _leftPressTime = 0;
                _leftPressStart = new();
            }

            if (MouseRight)
            {
                if (_rightPressTime == 0)
                    _rightPressStart = CurrentMouseState.Position();
                _rightPressTime++;
                if (_rightPressTime > 20)
                    KeepPressRight?.Invoke(null, _rightPressTime, _rightPressStart);
            }
            else
            {
                if (_rightPressTime <= 20 && _rightPressTime != 0)
                    RightClick?.Invoke(null, _rightPressTime, _rightPressStart);

                _rightPressTime = 0;
                _rightPressStart = new();
            }
        }

        public static bool IsPressed(Keys key)
        {
            return PreviousKeyState.IsKeyDown(key) && CurrentKeyState.IsKeyUp(key);
        }

        public static bool IsKeyDown(Keys key) => CurrentKeyState.IsKeyDown(key);

        public static bool IsKeyUp(Keys key) => CurrentKeyState.IsKeyUp(key);

        public static bool IsJustPress(Keys key)
        {
            return PreviousKeyState.IsKeyUp(key) && CurrentKeyState.IsKeyDown(key);
        }

        public static bool MouseLeft => CurrentMouseState.LeftButton == ButtonState.Pressed;

        public static bool MouseRight => CurrentMouseState.RightButton == ButtonState.Pressed;

        public static int GetDeltaWheelValue()
        {
            return CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
        }

        public static Rectangle GetMouseRectangle()
        {
            return new Rectangle(CurrentMouseState.X, CurrentMouseState.Y, 1, 1);
        }

        public static bool Shift => IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);

        public static bool Ctrl => IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

        public static bool Alt => IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
    }

    public class KeyEventArgs : EventArgs
    {
        public Keys KeyCode { get; private set; }

        public KeyEventArgs(Keys keyCode)
        {
            KeyCode = keyCode;
        }
    }

    public class CharacterEventArgs : EventArgs
    {
        public char Character { get; private set; }

        public CharacterEventArgs(char character)
        {
            Character = character;
        }
    }

    public class Clipboard
    {
        public static void SetClipboardText(string text)
        {
            OpenClipboard(IntPtr.Zero);

            EmptyClipboard();

            SetClipboardData(1, Marshal.StringToCoTaskMemUTF8(text));

            CloseClipboard();
        }

        public static string GetClipboardText()
        {
            OpenClipboard(IntPtr.Zero);

            var data = GetClipboardData(1);

            var text = Marshal.PtrToStringUTF8(data);

            CloseClipboard();

            return text;
        }

        [DllImport("user32.dll")]
        private extern static bool EmptyClipboard();

        [DllImport("user32.dll")]
        private extern static bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private extern static bool CloseClipboard();

        [DllImport("user32.dll")]
        private extern static IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private extern static IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
    }
}
