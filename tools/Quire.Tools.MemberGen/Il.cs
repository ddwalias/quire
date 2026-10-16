using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

/// <summary>One decoded instruction: an int, long, double or string constant, a metadata token, a local or
/// argument number, or branch targets as operand.</summary>
sealed record Instruction(int Offset, OpCode Op, object? Operand)
{
    public override string ToString() => Operand switch
    {
        null => Op.Name!,
        EntityHandle h => $"{Op.Name} 0x{MetadataTokens.GetToken(h):x8}",
        _ => $"{Op.Name} {Operand}",
    };
}

static class Il
{
    private static readonly Dictionary<ushort, OpCode> Codes = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null)!)
        .ToDictionary(o => unchecked((ushort)o.Value));

    /// <summary>The instructions of a body, with macro forms (<c>ldarg.0</c>, <c>ldc.i4.2</c>) expanded and short
    /// forms widened, so that patterns name each operation once.</summary>
    public static List<Instruction> Read(MethodBodyBlock body)
    {
        var il = body.GetILReader();
        var list = new List<Instruction>();
        while (il.RemainingBytes > 0)
        {
            int offset = il.Offset;
            byte first = il.ReadByte();
            var op = Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            object? operand = op.OperandType switch
            {
                OperandType.InlineNone => null,
                OperandType.ShortInlineBrTarget => (object)new[] { il.ReadSByte() },
                OperandType.InlineBrTarget => new[] { il.ReadInt32() },
                OperandType.InlineSwitch => Enumerable.Range(0, il.ReadInt32()).Select(_ => il.ReadInt32()).ToArray(),
                OperandType.ShortInlineI => op == OpCodes.Ldc_I4_S ? (int)il.ReadSByte() : il.ReadByte(),
                OperandType.InlineI => il.ReadInt32(),
                OperandType.InlineI8 => il.ReadInt64(),
                OperandType.ShortInlineR => (double)il.ReadSingle(),
                OperandType.InlineR => il.ReadDouble(),
                OperandType.ShortInlineVar => (int)il.ReadByte(),
                OperandType.InlineVar => (int)il.ReadUInt16(),
                OperandType.InlineString => MetadataTokens.UserStringHandle(il.ReadInt32()),
                _ => MetadataTokens.EntityHandle(il.ReadInt32()),
            };
            (op, operand) = Expand(op, operand);
            list.Add(new(offset, op, operand));
        }
        return list;
    }

    private static (OpCode, object?) Expand(OpCode op, object? operand)
    {
        string name = op.Name!;
        if (name.StartsWith("ldc.i4.") && name != "ldc.i4.s") return (OpCodes.Ldc_I4, name == "ldc.i4.m1" ? -1 : name[^1] - '0');
        if (name == "ldc.i4.s") return (OpCodes.Ldc_I4, operand);
        if (name == "ldc.r4") return (OpCodes.Ldc_R4, operand);
        foreach (var (family, wide) in new[] { ("ldarg", OpCodes.Ldarg), ("starg", OpCodes.Starg), ("ldloc", OpCodes.Ldloc), ("stloc", OpCodes.Stloc),
                     ("ldarga", OpCodes.Ldarga), ("ldloca", OpCodes.Ldloca) })
        {
            if (name == family + ".s") return (wide, operand);
            if (name.Length == family.Length + 2 && name.StartsWith(family + ".") && char.IsDigit(name[^1])) return (wide, name[^1] - '0');
        }
        return op.OperandType == OperandType.ShortInlineBrTarget
            ? (Codes.Values.Single(o => o.Name == name[..^2]), operand) : (op, operand);
    }
}
