using System;
using System.Collections.Generic;
using System.Text;

namespace TerraJS.JSEngine.API.Commands.Completion
{
    public readonly struct CommandToken
    {
        public CommandToken(string raw, string text, int start, int end, bool quoted, bool closed)
        {
            Raw = raw;

            Text = text;

            Start = start;

            End = end;

            Quoted = quoted;

            Closed = closed;
        }

        public readonly string Raw;

        public readonly string Text;

        public readonly int Start;

        public readonly int End;

        public readonly bool Quoted;

        public readonly bool Closed;

        public int Length => End - Start;

        public bool Contains(int cursor) => cursor >= Start && cursor <= End;

        public int LocalCursor(int cursor) => Math.Clamp(cursor - Start, 0, Length);

        public string RawPrefix(int cursor) => Raw[..LocalCursor(cursor)];

        public string RawSuffix(int cursor) => Raw[LocalCursor(cursor)..];
    }

    public static class CommandTokenizer
    {
        private const char Escape = '\\';

        public static List<CommandToken> Tokenize(string input)
        {
            var tokens = new List<CommandToken>();

            if (string.IsNullOrEmpty(input))
                return tokens;

            var index = 0;

            while (index < input.Length)
            {
                if (char.IsWhiteSpace(input[index]))
                {
                    index++;

                    continue;
                }

                tokens.Add(ReadToken(input, ref index));
            }

            return tokens;
        }

        private static CommandToken ReadToken(string input, ref int index)
        {
            var start = index;

            var builder = new StringBuilder();

            var quoted = false;

            var closed = true;

            var quote = '\0';

            var depth = 0;

            while (index < input.Length)
            {
                var c = input[index];

                if (c == Escape && index + 1 < input.Length)
                {
                    builder.Append(Unescape(input[index + 1]));

                    index += 2;

                    continue;
                }

                if (quote != '\0')
                {
                    if (c == quote)
                    {
                        quote = '\0';

                        closed = true;
                    }
                    else
                        builder.Append(c);

                    index++;

                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    quote = c;

                    quoted = true;

                    closed = false;

                    index++;

                    continue;
                }

                if (c == '[')
                    depth++;
                else if (c == ']' && depth > 0)
                    depth--;

                if (depth == 0 && char.IsWhiteSpace(c))
                    break;

                builder.Append(c);

                index++;
            }

            return new CommandToken(input[start..index], builder.ToString(), start, index, quoted, closed);
        }

        private static string Unescape(char c) => c switch
        {
            'n' => "\n",
            'r' => "\r",
            't' => "\t",
            '0' => "\0",
            '\\' => "\\",
            '"' => "\"",
            '\'' => "'",
            _ => Escape + c.ToString(),
        };
    }
}
