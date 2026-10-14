using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

/// <summary>
/// A same-size byte change inside one method body that still verifies: a negated branch, a swapped arithmetic or
/// comparison operator, a 0/1 constant swapped, or another constant moved by one. Applying it to a copy of the
/// assembly must make a direct comparison fail; a surviving mutant marks a branch, operator or constant no case
/// distinguishes.
/// </summary>
sealed record Mutant(int IlOffset, long FileOffset, string Original, string Replacement, string Description);

static class Mutants
{
    private static readonly Dictionary<ushort, ushort> Swaps = new()
    {
        // Short and long conditional branches, each to its negation.
        [0x2C] = 0x2D, [0x2D] = 0x2C, [0x2E] = 0x33, [0x33] = 0x2E, [0x2F] = 0x32, [0x32] = 0x2F,
        [0x30] = 0x31, [0x31] = 0x30, [0x34] = 0x37, [0x37] = 0x34, [0x35] = 0x36, [0x36] = 0x35,
        [0x39] = 0x3A, [0x3A] = 0x39, [0x3B] = 0x40, [0x40] = 0x3B, [0x3C] = 0x3F, [0x3F] = 0x3C,
        [0x3D] = 0x3E, [0x3E] = 0x3D, [0x41] = 0x44, [0x44] = 0x41, [0x42] = 0x43, [0x43] = 0x42,
        // add/sub, mul/div, rem→div, and/or, xor→or, shl/shr.
        [0x58] = 0x59, [0x59] = 0x58, [0x5A] = 0x5B, [0x5B] = 0x5A, [0x5D] = 0x5B,
        [0x5F] = 0x60, [0x60] = 0x5F, [0x61] = 0x60, [0x62] = 0x63, [0x63] = 0x62,
        // cgt/clt, cgt.un/clt.un.
        [0xFE02] = 0xFE04, [0xFE04] = 0xFE02, [0xFE03] = 0xFE05, [0xFE05] = 0xFE03,
    };

    public static List<Mutant> Find(PEReader pe, MethodDefinition method)
    {
        var found = new List<Mutant>();
        int rva = method.RelativeVirtualAddress;
        var section = pe.PEHeaders.SectionHeaders.FirstOrDefault(h => rva >= h.VirtualAddress && rva < h.VirtualAddress + h.VirtualSize);
        if (rva == 0 || section.VirtualSize == 0) return found;
        int bodyOffset = section.PointerToRawData + rva - section.VirtualAddress;
        var image = pe.GetEntireImage().GetContent();
        int code = bodyOffset + ((image[bodyOffset] & 0x3) == 0x2 ? 1 : 12);
        var il = pe.GetMethodBody(method.RelativeVirtualAddress).GetILReader();
        while (il.RemainingBytes > 0)
        {
            int at = il.Offset;
            byte first = il.ReadByte();
            ushort value = first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first;
            var op = StructureReader.Codes[value];
            int operand = il.Offset;
            Skip(ref il, op);
            void Add(byte[] original, byte[] replacement, string description, int offset) =>
                found.Add(new(at, code + offset, Convert.ToHexString(original), Convert.ToHexString(replacement), description));
            if (Swaps.TryGetValue(value, out ushort swap))
            {
                var other = StructureReader.Codes[swap];
                byte[] Bytes(ushort v) => v > 0xFF ? [0xFE, (byte)v] : [(byte)v];
                Add(Bytes(value), Bytes(swap), $"{op.Name} -> {other.Name}", at);
            }
            else if (value is >= 0x15 and <= 0x1E)
            {
                // ldc.i4.m1 .. ldc.i4.8: 0 and 1 swap (true/false must differ), others shift up (8 goes down).
                byte next = value switch { 0x16 => 0x17, 0x17 => 0x16, 0x1E => 0x1D, _ => (byte)(value + 1) };
                Add([(byte)value], [next], $"{op.Name} -> {StructureReader.Codes[next].Name}", at);
            }
            else if (value == 0x1F)
            {
                sbyte v = (sbyte)image[code + operand];
                Add([(byte)v], [(byte)(sbyte)(v + 1)], $"ldc.i4.s {v} -> {(sbyte)(v + 1)}", operand);
            }
            else if (value == 0x20)
            {
                int v = BitConverter.ToInt32(image.AsSpan(code + operand, 4));
                Add(BitConverter.GetBytes(v), BitConverter.GetBytes(unchecked(v + 1)), $"ldc.i4 {v} -> {unchecked(v + 1)}", operand);
            }
            else if (value == 0x23)
            {
                double v = BitConverter.ToDouble(image.AsSpan(code + operand, 8));
                double changed = v == 0 ? 1 : v * 2;
                Add(BitConverter.GetBytes(v), BitConverter.GetBytes(changed), $"ldc.r8 {v:R} -> {changed:R}", operand);
            }
        }
        return found;
    }

    private static void Skip(ref BlobReader il, OpCode op)
    {
        switch (op.OperandType)
        {
            case OperandType.InlineNone: break;
            case OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar: il.ReadByte(); break;
            case OperandType.InlineVar: il.ReadUInt16(); break;
            case OperandType.InlineI8 or OperandType.InlineR: il.ReadInt64(); break;
            case OperandType.InlineSwitch:
                int targets = il.ReadInt32();
                for (int i = 0; i < targets; i++) il.ReadInt32();
                break;
            default: il.ReadInt32(); break;
        }
    }
}
