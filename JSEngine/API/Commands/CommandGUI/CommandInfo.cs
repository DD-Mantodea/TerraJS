using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerraJS.JSEngine.API.Commands.Completion;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
{
    public enum InputState
    {
        Empty,
        Command,
        Parameter,
        Selector,
        Entity
    }

    public class CommandInfo
    {
        public InputState State { get; set; } = InputState.Empty;

        public string FullInput { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public List<string> Parameters { get; set; } = new();

        public string CurrentParameter { get; set; } = string.Empty;

        public int ParameterIndex { get; set; } = -1;

        public int CursorPosition { get; set; } = 0;

        public int RelativeCursorPosition { get; set; } = 0;

        public List<CommandToken> Tokens { get; set; } = new();

        public int CommandTokenIndex { get; set; } = -1;

        public int CurrentTokenIndex { get; set; } = -1;

        public CommandToken CurrentToken { get; set; }

        public string Prefix { get; set; } = string.Empty;

        public string Suffix { get; set; } = string.Empty;

        public string MatchPrefix { get; set; } = string.Empty;

        public bool Quoted { get; set; }

        public bool HasTrailingSpace { get; set; }

        public bool InBracket { get; set; }

        public string BracketHead { get; set; } = string.Empty;

        public int BracketStart { get; set; } = -1;

        public int BracketEnd { get; set; } = -1;

        public int SegmentStart { get; set; } = -1;

        public int SegmentEnd { get; set; } = -1;

        public bool CursorInValue { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public string AttributeOperator { get; set; } = string.Empty;

        public int ReplaceStart { get; set; } = 0;

        public int ReplaceLength { get; set; } = 0;

        public static CommandInfo Parse(string inputText, int cursorPosition) => CommandParser.Parse(inputText, cursorPosition);
    }

    public static class CommandParser
    {
        private static readonly Regex SelectorHead = new(@"^@[A-Za-z][A-Za-z0-9_]*$");

        private static readonly Regex EntityHead = new(@"^[A-Za-z0-9_]+:[A-Za-z0-9_]+$");

        private static readonly string[] Operators = [">=", "<=", "!=", "=", ">", "<"];

        private static string _cachedInput;

        private static int _cachedCursor = -1;

        private static CommandInfo _cached;

        public static CommandInfo Parse(string inputText, int cursorPosition)
        {
            if (_cached is not null && _cachedInput == inputText && _cachedCursor == cursorPosition)
                return _cached;

            var info = Build(inputText, cursorPosition);

            _cachedInput = inputText;

            _cachedCursor = cursorPosition;

            _cached = info;

            return info;
        }

        private static CommandInfo Build(string inputText, int cursorPosition)
        {
            var info = new CommandInfo
            {
                FullInput = inputText ?? string.Empty,
                CursorPosition = Math.Clamp(cursorPosition, 0, (inputText ?? string.Empty).Length),
            };

            if (string.IsNullOrEmpty(info.FullInput) || !info.FullInput.StartsWith('/'))
            {
                info.State = InputState.Empty;

                return info;
            }

            var tokens = CommandTokenizer.Tokenize(info.FullInput);

            if (tokens.Count > 0 && tokens[0].Start == 0 && tokens[0].Raw.StartsWith('/'))
                tokens[0] = new CommandToken(tokens[0].Raw[1..], tokens[0].Text.StartsWith('/') ? tokens[0].Text[1..] : tokens[0].Text, 1, tokens[0].End, tokens[0].Quoted, tokens[0].Closed);

            tokens.RemoveAll(token => token.Length == 0);

            info.Tokens = tokens;

            var (index, token) = Locate(tokens, info.CursorPosition);

            info.CurrentTokenIndex = index;

            info.CurrentToken = token;

            info.Prefix = token.RawPrefix(info.CursorPosition);

            info.Suffix = token.RawSuffix(info.CursorPosition);

            info.MatchPrefix = StripQuote(info.Prefix);

            info.Quoted = token.Quoted;

            info.HasTrailingSpace = info.FullInput.Length > 0 && char.IsWhiteSpace(info.FullInput[^1]);

            info.RelativeCursorPosition = token.LocalCursor(info.CursorPosition);

            info.ReplaceStart = token.Length == 0 ? info.CursorPosition : token.Start;

            info.ReplaceLength = token.Length;

            info.CommandTokenIndex = tokens.Count > 0 ? 0 : -1;

            info.Command = tokens.Count > 0 ? tokens[0].Text : string.Empty;

            for (var i = 1; i < tokens.Count; i++)
                info.Parameters.Add(tokens[i].Text);

            if (index == 0)
            {
                info.State = InputState.Command;

                info.ParameterIndex = -1;

                if (info.ReplaceStart < 1)
                {
                    info.ReplaceStart = 1;

                    info.ReplaceLength = 0;
                }

                return info;
            }

            info.State = InputState.Parameter;

            info.ParameterIndex = index - 1;

            info.CurrentParameter = token.Text;

            AnalyzeBracket(info, token);

            return info;
        }

        private static (int Index, CommandToken Token) Locate(List<CommandToken> tokens, int cursor)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Contains(cursor))
                    return (i, tokens[i]);
            }

            var index = 0;

            while (index < tokens.Count && tokens[index].End <= cursor)
                index++;

            return (index, new CommandToken(string.Empty, string.Empty, cursor, cursor, false, true));
        }

        private static void AnalyzeBracket(CommandInfo info, CommandToken token)
        {
            var raw = token.Raw;

            var local = token.LocalCursor(info.CursorPosition);

            var open = -1;

            var depth = 0;

            for (var i = 0; i < raw.Length; i++)
            {
                if (raw[i] == '[')
                {
                    depth++;

                    if (i < local)
                        open = i;
                }
                else if (raw[i] == ']' && depth > 0)
                    depth--;
            }

            if (open < 0)
                return;

            info.InBracket = true;

            info.BracketHead = raw[..open];

            info.BracketStart = token.Start + open + 1;

            var close = raw.Length;

            for (var i = open + 1; i < raw.Length; i++)
            {
                if (raw[i] == ']' && i >= local)
                {
                    close = i;

                    break;
                }
            }

            info.BracketEnd = token.Start + close;

            var content = raw[(open + 1)..close];

            var offset = Math.Clamp(local - open - 1, 0, content.Length);

            var segStart = 0;

            var segEnd = content.Length;

            var segDepth = 0;

            for (var i = 0; i < content.Length; i++)
            {
                var c = content[i];

                if (c == '[')
                {
                    segDepth++;

                    continue;
                }

                if (c == ']')
                {
                    if (segDepth > 0)
                        segDepth--;

                    continue;
                }

                if (c != ',' || segDepth > 0)
                    continue;

                if (offset <= i)
                {
                    segEnd = i;

                    break;
                }

                segStart = i + 1;
            }

            var segment = content[segStart..segEnd];

            var segmentOffset = Math.Clamp(offset - segStart, 0, segment.Length);

            info.SegmentStart = token.Start + open + 1 + segStart;

            info.SegmentEnd = token.Start + open + 1 + segEnd;

            info.ReplaceStart = info.SegmentStart;

            info.ReplaceLength = info.SegmentEnd - info.SegmentStart;

            ApplyState(info);

            var operatorIndex = FindOperator(segment);

            if (operatorIndex < 0)
            {
                info.MatchPrefix = StripQuote(segment[..segmentOffset]);

                return;
            }

            var operatorLength = OperatorLength(segment, operatorIndex);

            var valueStart = operatorIndex + operatorLength;

            info.AttributeName = segment[..operatorIndex].Trim();

            info.AttributeOperator = segment.Substring(operatorIndex, operatorLength);

            if (segmentOffset <= operatorIndex)
            {
                info.ReplaceLength = operatorIndex;

                info.MatchPrefix = StripQuote(segment[..segmentOffset]);

                return;
            }

            info.CursorInValue = true;

            info.ReplaceStart = token.Start + open + 1 + segStart + valueStart;

            info.ReplaceLength = Math.Max(0, segEnd - segStart - valueStart);

            info.MatchPrefix = StripQuote(segment[valueStart..Math.Clamp(segmentOffset, valueStart, segment.Length)]);
        }

        private static void ApplyState(CommandInfo info)
        {
            if (SelectorHead.IsMatch(info.BracketHead))
                info.State = InputState.Selector;
            else if (EntityHead.IsMatch(info.BracketHead))
                info.State = InputState.Entity;
        }

        private static int FindOperator(string segment)
        {
            var result = -1;

            foreach (var op in Operators)
            {
                var index = segment.IndexOf(op, StringComparison.Ordinal);

                if (index < 0)
                    continue;

                if (result < 0 || index < result)
                    result = index;
            }

            return result;
        }

        private static int OperatorLength(string segment, int index)
        {
            if (index + 1 >= segment.Length)
                return 1;

            var pair = segment.Substring(index, 2);

            return pair is ">=" or "<=" or "!=" ? 2 : 1;
        }

        private static string StripQuote(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text[0] is '"' or '\'' ? text[1..] : text;
        }
    }
}
