using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TerraJS.JSEngine;

namespace TerraJS.Contents.Utils
{
    public class DelegateUtils
    {
        public static Type GetDelegateType(bool isFunc, int parameterCount)
        {
            if (isFunc)
            {
                return (parameterCount + 1) switch
                {
                    > 17 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    <= 0 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    <= 17 => Type.GetType($"System.Func`{parameterCount + 1}")
                };
            }
            else
            {
                return parameterCount switch
                {
                    0 => typeof(Action),
                    > 16 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    < 0 => throw new NotSupportedException($"不支持具有 {parameterCount} 个参数的方法"),
                    >= 0 => Type.GetType($"System.Action`{parameterCount}")
                };
            }
        }
    
        public static Delegate CreateHook(Delegate @delegate, Type hookType)
        {
            var method = @delegate.Method;

            var parameters = RegistryUtils.Parameters2Types(method.GetParameters());

            var hookMethod = TJSEngine.Engine.TypeConverter.Convert(@delegate, hookType, CultureInfo.InvariantCulture);

            var paramExpressions = new ParameterExpression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                paramExpressions[i] = Expression.Parameter(parameters[i], parameters[i].Name);

            return @delegate;
        }

        public static void DumpDelegate(Delegate del, string filePath)
        {
            if (del == null) return;

            MethodInfo method = del.Method;

            Module module = method.Module;

            MethodBody body = method.GetMethodBody();

            if (body == null)
            {
                File.WriteAllText(filePath, "No method body available.");

                return;
            }

            byte[] ilBytes = body.GetILAsByteArray();

            StringBuilder sb = new();

            sb.AppendLine($"Target Method: {method.DeclaringType?.FullName}.{method.Name}");

            sb.AppendLine("----------------------------------------------------------------");

            int index = 0;
            while (index < ilBytes.Length)
            {
                int startPosition = index;

                byte opByte = ilBytes[index++];

                OpCode opCode;

                if (opByte == 0xFE && index < ilBytes.Length)
                {
                    byte secondaryByte = ilBytes[index++];

                    short shortVal = (short)((opByte << 8) | secondaryByte);

                    opCode = GetOpCodeFromShort(shortVal);
                }
                else
                    opCode = GetOpCodeFromByte(opByte);

                sb.Append($"IL_{startPosition:X4}: {opCode.Name.PadRight(12)}");

                int operandSize = GetOperandSize(opCode.OperandType);

                if (operandSize > 0 && index + operandSize <= ilBytes.Length)
                {
                    byte[] operandBytes = new byte[operandSize];

                    Array.Copy(ilBytes, index, operandBytes, 0, operandSize);

                    index += operandSize;

                    string translatedOperand = TranslateOperand(del, opCode.OperandType, operandBytes, module, startPosition + opCode.Size);

                    sb.Append($" {translatedOperand}");
                }

                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        private static string TranslateOperand(Delegate del, OperandType type, byte[] bytes, Module module, int nextInstructionAddr)
        {
            try
            {
                switch (type)
                {
                    case OperandType.ShortInlineI:
                        return ((sbyte)bytes[0]).ToString();

                    case OperandType.InlineI:
                        return BitConverter.ToInt32(bytes, 0).ToString();

                    case OperandType.InlineI8:
                        return BitConverter.ToInt64(bytes, 0).ToString();

                    case OperandType.ShortInlineR:
                        return BitConverter.ToSingle(bytes, 0).ToString();

                    case OperandType.InlineR:
                        return BitConverter.ToDouble(bytes, 0).ToString();

                    case OperandType.ShortInlineBrTarget:
                        sbyte shortOffset = (sbyte)bytes[0];

                        return $"-> IL_{(nextInstructionAddr + shortOffset):X4}";

                    case OperandType.InlineBrTarget:
                        int longOffset = BitConverter.ToInt32(bytes, 0);

                        return $"-> IL_{(nextInstructionAddr + longOffset):X4}";

                    case OperandType.InlineMethod:
                        int methodToken = BitConverter.ToInt32(bytes, 0);

                        MethodBase calledMethod = module.ResolveMethod(methodToken);

                        return $"[Method] {calledMethod.DeclaringType?.FullName}.{calledMethod.Name}()";

                    case OperandType.InlineType:
                        int typeToken = BitConverter.ToInt32(bytes, 0);

                        Type resolvedType = module.ResolveType(typeToken);

                        return $"[Type] {resolvedType.FullName}";

                    case OperandType.InlineField:
                        int fieldToken = BitConverter.ToInt32(bytes, 0);

                        FieldInfo resolvedField = module.ResolveField(fieldToken);

                        return $"[Field] {resolvedField.DeclaringType?.FullName}.{resolvedField.Name}";

                    case OperandType.InlineString:
                        int stringToken = BitConverter.ToInt32(bytes, 0);

                        return $"\"{module.ResolveString(stringToken)}\"";

                    case OperandType.ShortInlineVar:
                        return $"idx_{bytes[0]}";

                    case OperandType.InlineVar:
                        return $"idx_{BitConverter.ToUInt16(bytes, 0)}";

                    default:
                        return "0x" + BitConverter.ToString(bytes).Replace("-", "");
                }
            }
            catch
            {
                return "0x" + BitConverter.ToString(bytes).Replace("-", "") + " (Failed to resolve)";
            }
        }

        private static OpCode GetOpCodeFromByte(byte opByte)
        {
            foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode op = (OpCode)field.GetValue(null)!;
                if (op.Size == 1 && op.Value == opByte) return op;
            }
            return OpCodes.Nop;
        }

        private static OpCode GetOpCodeFromShort(short opShort)
        {
            foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OpCode op = (OpCode)field.GetValue(null)!;
                if (op.Size == 2 && op.Value == opShort) return op;
            }
            return OpCodes.Nop;
        }

        private static int GetOperandSize(OperandType type)
        {
            return type switch
            {
                OperandType.InlineNone => 0,
                OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
                OperandType.InlineVar => 2,
                OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineMethod or OperandType.InlineSig or OperandType.InlineString or OperandType.InlineSwitch or OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR => 4,
                OperandType.InlineI8 or OperandType.InlineR => 8,
                _ => 0,
            };
        }
    }
}
