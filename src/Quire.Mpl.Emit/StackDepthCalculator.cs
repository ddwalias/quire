using System;
using System.Collections.Generic;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class StackDepthCalculator
{
    private static readonly int _defaultMaxStackDepth = 48;

    public StackDepthCalculator()
    {
    }

    public static int CalculateMaxStackDepth(List<ILInstruction> instructions)
    {
        return Math.Max(CalculateMaxStackDepthDefault(instructions), CalculateMaxStackDepthOptimistic(instructions));
    }

    private static int CalculateMaxStackDepthDefault(List<ILInstruction> instructions)
    {
        return _defaultMaxStackDepth;
    }

    private static int CalculateMaxStackDepthPushOnly(List<ILInstruction> instructions)
    {
        int num = 0;
        int maxDepth = 0;
        foreach (ILInstruction instruction in instructions)
        {
            OpCode opCode = instruction.OpCode;
            object operand = instruction.Operand;
            int pushCount = opCode.StackBehaviourPush.GetPushCount(operand);
            num += pushCount;
            maxDepth = Math.Max(maxDepth, num);
        }
        return maxDepth;
    }

    private static int CalculateMaxStackDepthOptimistic(List<ILInstruction> instructions)
    {
        int num = 0;
        int maxDepth = 0;
        foreach (ILInstruction instruction in instructions)
        {
            OpCode opCode = instruction.OpCode;
            object operand = instruction.Operand;
            int popCount = opCode.StackBehaviourPop.GetPopCount(opCode, operand);
            num -= popCount;
            if (instruction.CallCommand == ILCallsEncoding.eMarkLabel || instruction.CallCommand == ILCallsEncoding.eBeginCatchBlock || instruction.CallCommand == ILCallsEncoding.eEndExceptionBlock)
            {
                num = maxDepth;
            }
            int pushCount = opCode.StackBehaviourPush.GetPushCount(operand);
            num += pushCount;
            maxDepth = Math.Max(maxDepth, num);
        }
        return maxDepth * 2 + 1;
    }
}
