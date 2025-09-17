using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TerraJS.API.Commands.CommandGUI;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.UI.IME;
using TerraJS.Contents.Utils;
using TerraJS.Hooks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace TerraJS.Contents.UI.Chat
{
    [RegisterUI("ChatBox")]
    public class ChatBox : SizeContainer
    {
        public ChatBox()
        {
            TextBox = new TextBox(200, 30, charSpacing: 1.25f).Join(this);

            Timer = new Timer().Join(this);

            Visible = false;

            UserInput.KeyJustPress += KeyJustPress;

            UserInput.KeyKeepPress += KeyKeepPress;
        }

        public static ChatBox Instance => UISystem.GetUIInstance<ChatBox>("ChatBox");

        public TextBox TextBox;

        public Timer Timer;

        public string immStr;

        public bool JustClose = false;

        private List<string> _messageHistory = [];

        private int _messageIndex;

        public override bool Visible { get; set; }

        public override void Update(GameTime gameTime)
        {
            RelativePosition = new(78, Main.screenHeight - 36);

            TextBox._width = Main.screenWidth - 300;

            base.Update(gameTime);

            if (!Visible)
                TextBox.Active = false;

            if (JustClose)
            {
                if (Timer[0] == 15)
                {
                    Timer[0] = 0;

                    JustClose = false;
                }

                Timer[0]++;
            }
        }

        private void KeyJustPress(object sender, KeyEventArgs e)
        {
            if (Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        Close();

                        break;

                    case Keys.Enter:
                        if (TextBox.Active)
                            SendMessage(TextBox.Text);
                        else
                            TextBox.Active = true;

                        break;

                    case Keys.Up:
                        if (TextBox.Active)
                            SwitchPrevMessage();

                        break;

                    case Keys.Down:
                        if (TextBox.Active)
                            SwitchNextMessage();

                        break;
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter && Main.hasFocus && !UserInput.Alt)
                {
                    if (Main.InGameUI.IsVisible || Main.ingameOptionsWindow)
                        return;

                    if (Main.chatRelease && !Main.drawingPlayerChat && !Main.editSign && !Main.editChest && !Main.gameMenu && !UserInput.Esc)
                        Open();

                    Main.chatRelease = false;
                }
            }
        }

        private void KeyKeepPress(object sender, KeyEventArgs e)
        {
            if (Visible)
            {
                switch (e.KeyCode)
                {
                    
                }
            }
        }

        private void Close()
        {
            SoundEngine.PlaySound(SoundID.MenuClose);

            Visible = false;

            TextBox.Active = false;

            JustClose = true;
        }

        private void Open()
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);

            Visible = true;

            TextBox.Active = true;

            Main.inputTextEscape = false;
        }

        private void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            _messageHistory.Add(message);

            _messageIndex = _messageHistory.Count;

            if (!CommandLoaderHook.HandleCommandHook(null, message, new ChatCommandCaller()))
            {
                var msg = ChatManager.Commands.CreateOutgoingMessage(message);

                switch (Main.netMode)
                {
                    case NetmodeID.MultiplayerClient:
                        ChatHelper.SendChatMessageFromClient(msg);
                        
                        break;

                    case NetmodeID.SinglePlayer:
                        ChatManager.Commands.ProcessIncomingMessage(msg, Main.myPlayer);

                        break;
                }
            }

            TextBox.ClearText();
        }

        private void SwitchPrevMessage()
        {
            if (_messageHistory.Count == 0 || _messageIndex == 0 || CommandPanel.Instance.CompletionsContainer.Completions.Count > 0)
                return;

            _messageIndex--;

            TextBox.Text = _messageHistory[_messageIndex];

            TextBox.CursorTo(TextBox.Text.Length);
        }

        private void SwitchNextMessage()
        {
            if (_messageHistory.Count == 0 || _messageIndex == _messageHistory.Count || CommandPanel.Instance.CompletionsContainer.Completions.Count > 0)
                return;

            _messageIndex++;

            if (_messageIndex == _messageHistory.Count)
                TextBox.Text = "";
            else
                TextBox.Text = _messageHistory[_messageIndex];

            TextBox.CursorTo(TextBox.Text.Length);
        }

        public void ClearMessage()
        {
            _messageHistory = [];

            _messageIndex = 0;

            TextBox.Text = "";
        }
    }
}
