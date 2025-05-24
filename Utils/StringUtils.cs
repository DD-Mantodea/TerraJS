using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria.UI.Chat;
using static Terraria.GameContent.UI.Chat.PlainTagHandler;

namespace TerraJS.Utils
{
    public class StringUtils
    {
        public static List<TextSnippet> ParseMessage(string input, Color baseColor)
        {
            input = input.Replace("\r", "");

            var matchCollection = ChatManager.Regexes.Format.Matches(input);

            List<TextSnippet> snippets = [];
            var inputIndex = 0;

            foreach (var match in matchCollection.Cast<Match>())
            {
                if (match.Index > inputIndex)
                    snippets.Add(new TextSnippet(input[inputIndex..match.Index], baseColor));

                inputIndex = match.Index + match.Length;

                var tag = match.Groups["tag"].Value;
                var text = match.Groups["text"].Value;
                var options = match.Groups["options"].Value;

                var handler = typeof(ChatManager).GetMethod("GetHandler", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, [ tag ]) as ITagHandler;

                if (handler is null)
                {
                    snippets.Add(new TextSnippet(text, baseColor));
                }
                else
                {
                    var snippet = handler.Parse(text, baseColor, options);
                    snippet.TextOriginal = match.ToString();
                    snippets.Add(snippet);
                }
            }

            if (input.Length > inputIndex) 
                snippets.Add(new TextSnippet(input[inputIndex..], baseColor));

            return snippets;
        }
    }
}
