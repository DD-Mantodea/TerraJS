using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
{
    public class CompletionsContainer : ColumnContainer
    {
        public int SelectedCompletionIndex = 0;

        public List<string> Completions => [.. Children.Cast<SizeContainer>().Select(container => {
            var text = container.Children[0] as UIText;

            return SnippetUtils.ParseBack(text.Snippets);
        })];

        public string ChatText => ChatBox.Instance.TextBox.Text;

        public string CurrentInput => ChatText.StartsWith("/") ? ChatText.Substring(1) : "";

        public string[] Args => CurrentInput.Split(" ", StringSplitOptions.RemoveEmptyEntries);

        public void RebuildCompletions(IEnumerable<string> completions)
        {
            int maxWidth = 0;

            foreach (var text in completions)
            {
                var cmdLine = new SizeContainer(0, 36)
                {
                    BackgroundColor = Color.Gray * 0.7f
                }.Join(this);

                var content = new UIText("YaHei", text: text, fontSize: 22).Join(cmdLine);

                content.RelativePosition = new(8, 0);

                content.TextVerticalMiddle = true;

                content.VerticalMiddle = true;

                maxWidth = Math.Max(maxWidth, Math.Clamp(content.Width + 8, 300, int.MaxValue));
            }

            foreach (var i in Children)
                i.SetSize(maxWidth, 36);

            SelectedCompletionIndex = 0;
        }

        public List<ModCommand> GetMatchingCommands()
        {
            var allCommands = GetAvailableCommands();

            var matchingCommands = new List<ModCommand>();

            if (Args.Length <= 1)
                matchingCommands = [.. allCommands
                    .Where(cmd => cmd.Command.StartsWith(Args.Length == 0 ? "" : Args[0], StringComparison.OrdinalIgnoreCase))
                    .OrderBy(cmd => cmd.Command)];
            else
                matchingCommands = [.. allCommands
                    .Where(cmd => {
                        if(cmd is TJSCommand tjscmd)
                            return tjscmd.TryGetArgumentsText(Args[1..], out var _) && tjscmd.Command.StartsWith(Args[0], StringComparison.OrdinalIgnoreCase);
                        return false;
                    })
                    .OrderBy(cmd => cmd.Command)];

            SelectedCompletionIndex = 0;

            return matchingCommands;
        }

        public List<ModCommand> GetAvailableCommands()
        {
            List<ModCommand> commands = [];

            var allCommands = typeof(CommandLoader).GetField("Commands", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IDictionary<string, List<ModCommand>>;

            foreach (var list in allCommands.Values)
            {
                foreach (var command in list)
                {
                    if (CommandLoader.Matches(command.Type, CommandType.Chat))
                        commands.Add(command);
                }
            }

            return commands;
        }

        public override void DrawChildren(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var start = SelectedCompletionIndex <= 5 ? 0 : SelectedCompletionIndex - 5;

            var end = Children.Count <= 6 ? Children.Count : start + 6;

            foreach (var component in Children[start..end])
                component.DrawSelf(spriteBatch, gameTime);
        }

        public override void UpdateChildren(GameTime gameTime)
        {
            _height = 0;

            var start = SelectedCompletionIndex <= 5 ? 0 : SelectedCompletionIndex - 5;

            var end = Children.Count <= 6 ? Children.Count : start + 6;

            foreach (var component in Children)
            {
                var index = Children.IndexOf(component);

                if (component.Visible && index >= start && index < end)
                    SetChildRelativePos(component);

                component.Position = component.RelativePosition + Position;

                component.Update(gameTime);

                if (index == SelectedCompletionIndex)
                    component.BackgroundColor = Color.LightGray * 0.7f;
                else
                    component.BackgroundColor = Color.Gray * 0.7f;
            }
        }
    }
}
