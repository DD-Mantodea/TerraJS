using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FontStashSharp;
using Microsoft.Xna.Framework;
using TerraJS.Contents.UI;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Chat.TextSnippnts;

namespace TerraJS.Contents.Utils
{
    public class SnippetUtils
    {
        public static Dictionary<string, Func<string, TextSnippet>> Parsers = [];

        public static Regex Letter => new("[a-z]");

        public static List<TextSnippet> ParseMessage(string input)
        {
            var result = new List<TextSnippet>();

            var lastSnippet = 0;

            var index = 0;

            while (index < input.Length)
            {
                var c = input[index];

                if (c == '[')
                {
                    var identifier = "";

                    var len = 1;

                    while (Letter.IsMatch(input[index + len].ToString()))
                    {
                        identifier += input[index + len];

                        len++;
                    }

                    if (Parsers.TryGetValue(identifier, out var parser))
                    {
                        while (input[index + len] != ']')
                        {
                            identifier += input[index + len];

                            len++;
                        }

                        var value = parser(identifier);

                        if (value != null)
                        {
                            if (lastSnippet != index)
                                result.Add(new PlainTextSnippet(input[lastSnippet..index]));

                            result.Add(value);

                            index = index + len + 1;

                            lastSnippet = index;
                        }
                        else
                        {
                            index++;
                        }
                    }
                    else
                    {
                        index++;

                        if (index == input.Length)
                            result.Add(new PlainTextSnippet(input[lastSnippet..index]));
                    }
                }
                else
                {
                    index++;

                    if (index == input.Length)
                        result.Add(new PlainTextSnippet(input[lastSnippet..index]));
                }
            }

            return result;
        }

        public static string ParseBack(List<TextSnippet> snippets)
        {
            return string.Join("", snippets.Select(s => s.OriginalText));
        }

        public static Vector2 GetSize(List<TextSnippet> snippets, TerraJSFont font)
        {
            var size = new Vector2();

            foreach (var snippet in snippets)
            {
                var snippetSize = snippet.GetSize(font);

                size.X += snippetSize.X;

                size.Y = Math.Max(size.Y, snippetSize.Y);
            }

            return size;
        }

        public static Vector2 GetSize(string originalText, TerraJSFont font)
        {
            return GetSize(ParseMessage(originalText), font);
        }

        public static string GetPlainText(List<TextSnippet> snippets)
        {
            return string.Join("", snippets.Select(s => s.ToString()));
        }

        public static string GetPlainText(string originalText)
        {
            return GetPlainText(ParseMessage(originalText));
        }
    }
}
