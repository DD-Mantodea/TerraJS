using System;
using System.Collections.Generic;
using System.Linq;

namespace TerraJS.JSEngine.API.Commands.CommandGUI
{
    public enum InputState
    {
        Empty,          // 空输入
        Command,        // 正在输入指令关键字
        Parameter       // 正在输入参数
    }

    public class CommandInfo
    {
        public InputState State { get; set; } = InputState.Empty;
        public string FullInput { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public List<string> Parameters { get; set; } = new();
        public string CurrentParameter { get; set; } = string.Empty;
        public int ParameterIndex { get; set; } = -1;          // 当前正在编辑的参数索引
        public int CursorPosition { get; set; } = 0;           // 原始光标位置
        public int RelativeCursorPosition { get; set; } = 0;   // 相对于当前参数的光标位置
        public bool IsAtParameterStart { get; set; } = false;  // 光标是否在参数开头
        public bool IsAtParameterEnd { get; set; } = false;    // 光标是否在参数结尾

        public static CommandInfo Parse(string inputText, int cursorPosition) => CommandParser.Parse(inputText, cursorPosition);
    }

    public class CommandParser
    {
        public static CommandInfo Parse(string inputText, int cursorPosition)
        {
            var result = new CommandInfo
            {
                FullInput = inputText ?? string.Empty,
                CursorPosition = cursorPosition
            };

            if (string.IsNullOrEmpty(inputText))
            {
                result.State = InputState.Empty;
                return result;
            }

            // 检查是否以斜杠开头
            if (!inputText.StartsWith("/"))
            {
                // 如果不是指令格式，可以根据需要处理
                result.State = InputState.Empty;
                return result;
            }

            // 移除开头的斜杠
            string content = inputText.Substring(1);
            int adjustedCursor = cursorPosition - 1; // 调整光标位置（移除斜杠的影响）

            // 分割输入内容
            var parts = SplitWithCursorTracking(content, adjustedCursor);

            if (parts.Count == 0)
            {
                result.State = InputState.Command;
                return result;
            }

            // 第一个部分总是关键字
            result.Command = parts[0].Text;

            if (parts.Count == 1)
            {
                // 只有关键字部分
                var keywordPart = parts[0];

                if (adjustedCursor <= keywordPart.EndIndex)
                {
                    // 光标在关键字范围内
                    result.State = InputState.Command;
                    result.RelativeCursorPosition = adjustedCursor - keywordPart.StartIndex;
                }
                else if (adjustedCursor > keywordPart.EndIndex && content[adjustedCursor - 1] == ' ')
                {
                    // 光标在关键字后的空格位置，准备输入参数
                    result.State = InputState.Parameter;
                    result.ParameterIndex = 0;
                    result.IsAtParameterStart = true;
                }
            }
            else
            {
                // 有关键字和参数
                result.Parameters = parts.Skip(1).Select(p => p.Text).ToList();

                // 查找光标所在的part
                var currentPart = parts.FirstOrDefault(p =>
                    adjustedCursor >= p.StartIndex && adjustedCursor <= p.EndIndex + 1); // +1 允许在结尾后一个位置

                if (currentPart != null)
                {
                    if (currentPart.Index == 0)
                    {
                        // 光标在关键字部分
                        result.State = InputState.Command;
                        result.RelativeCursorPosition = adjustedCursor - currentPart.StartIndex;
                    }
                    else
                    {
                        // 光标在参数部分
                        result.State = InputState.Parameter;
                        result.ParameterIndex = currentPart.Index - 1; // 转换为参数索引
                        result.CurrentParameter = currentPart.Text;
                        result.RelativeCursorPosition = adjustedCursor - currentPart.StartIndex;

                        // 检查光标位置特性
                        result.IsAtParameterStart = adjustedCursor == currentPart.StartIndex;
                        result.IsAtParameterEnd = adjustedCursor == currentPart.EndIndex + 1;
                    }
                }
                else if (adjustedCursor > parts.Last().EndIndex + 1)
                {
                    // 光标在所有部分之后，准备输入新参数
                    result.State = InputState.Parameter;
                    result.ParameterIndex = parts.Count - 1;
                    result.IsAtParameterStart = true;
                }
            }

            return result;
        }

        private static List<TextPart> SplitWithCursorTracking(string text, int cursorPosition)
        {
            var parts = new List<TextPart>();
            int startIndex = 0;
            int currentIndex = 0;
            int partIndex = 0;
            bool inWord = false;

            while (currentIndex < text.Length)
            {
                if (char.IsWhiteSpace(text[currentIndex]))
                {
                    if (inWord)
                    {
                        // 结束当前单词
                        parts.Add(new TextPart
                        {
                            Index = partIndex++,
                            Text = text.Substring(startIndex, currentIndex - startIndex),
                            StartIndex = startIndex,
                            EndIndex = currentIndex - 1
                        });
                        inWord = false;
                    }
                    currentIndex++;
                }
                else
                {
                    if (!inWord)
                    {
                        // 开始新单词
                        startIndex = currentIndex;
                        inWord = true;
                    }
                    currentIndex++;
                }
            }

            // 处理最后一个单词
            if (inWord)
            {
                parts.Add(new TextPart
                {
                    Index = partIndex,
                    Text = text.Substring(startIndex, currentIndex - startIndex),
                    StartIndex = startIndex,
                    EndIndex = currentIndex
                });
            }

            return parts;
        }

        private class TextPart
        {
            public int Index { get; set; }
            public string Text { get; set; } = string.Empty;
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }
    }
}
