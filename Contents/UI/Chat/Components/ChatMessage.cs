using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Chat.TextSnippnts;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using Terraria;
using static System.Net.Mime.MediaTypeNames;

namespace TerraJS.Contents.UI.Chat
{
    public class ChatMessage : ColumnContainer
    {
        public ChatMessage(string message, string player, Color color, bool local)
        {
            Font = TerraJS.FontManager["YaHei", 22];

            SetMessage(message, player, color, local);

            Timer = new(1);

            _width = MessageBox.Instance.Width;

            _height = (int)Children.Sum(c => c.Size.Y);
        }
        
        public Timer Timer;

        public override bool Visible => Timer[0] < 600 || MessageBox.Instance.Visible;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Timer[0] < 600)
                Timer[0]++;
        }

        public void SetMessage(string message, string player, Color color, bool local)
        {
            Children.Clear();

            var text = $"[{(string.IsNullOrEmpty(player) ? "System" : player)}]: {message}";

            var maxWidth = MessageBox.Instance.Width - 10;

            var snippets = SnippetUtils.ParseMessage(text);
            
            var index = 0;

            if (color != Color.White)
            {
                while (index < snippets.Count)
                {
                    var snippet = snippets[index];

                    if (snippet is PlainTextSnippet plain)
                    {
                        var plainText = plain.OriginalText;

                        snippets[index] = new ColorTextSnippet(plainText, $"[c/{color.ToHexString()}:{plainText}]", color);
                    }

                    index++;
                }

                index = 0;
            }

            var width = 0;

            var start = 0;

            while (index < snippets.Count)
            {
                var snippet = snippets[index];

                if (width + snippet.GetSize(Font).X > maxWidth)
                {
                    var split = snippet.SplitByWidth(Font, maxWidth - width);

                    if (split.Count == 2)
                    {
                        List<TextSnippet> subSnippets = [..snippets[start..index], split[0]];

                        new UIText(Font, subSnippets).Join(this).SetRelativePos(new(4, 0));

                        snippets[index] = split[1];
                    }
                    else
                    {
                        var subSnippets = snippets[start..index];

                        new UIText(Font, subSnippets).Join(this).SetRelativePos(new(4, 0));
                    }

                    width = 0;

                    start = index;
                }
                else
                {
                    width += (int)snippet.GetSize(Font).X;

                    index++;

                    if (index == snippets.Count)
                    {
                        var subSnippets = snippets[start..index];

                        new UIText(Font, subSnippets).Join(this).SetRelativePos(new(4, 0));
                    }
                }
            }
        }
    }
}
