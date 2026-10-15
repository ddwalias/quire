using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

/// <summary>
/// A same-size byte change inside one method body that still verifies: a negated branch, a swapped arithmetic or
/// comparison operator, a 0/1 constant swapped, or another constant changed. Applying it to a copy of the
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
        // cgt/clt, cgt.un/clt.un; ceq (== and !=) becomes cgt.un.
        [0xFE02] = 0xFE04, [0xFE04] = 0xFE02, [0xFE03] = 0xFE05, [0xFE05] = 0xFE03, [0xFE01] = 0xFE03,
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
        var md = pe.GetMetadataReader();
        bool readsCapacity = ReadsCapacity(il, md);
        while (il.RemainingBytes > 0)
        {
            int at = il.Offset;
            byte first = il.ReadByte();
            ushort value = first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first;
            var op = StructureReader.Codes[value];
            int operand = il.Offset;
            Skip(ref il, op);
            if (!readsCapacity && value is >= 0x15 and <= 0x20 && Constant(value, image, code + operand) is >= 0 and <= 1 << 20
                && FeedsCapacity(il, md)) continue;
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
            else if (value == 0x21)
            {
                long v = BitConverter.ToInt64(image.AsSpan(code + operand, 8));
                Add(BitConverter.GetBytes(v), BitConverter.GetBytes(unchecked(v + 1)), $"ldc.i8 {v} -> {unchecked(v + 1)}", operand);
            }
            else if (value == 0x22)
            {
                float v = BitConverter.ToSingle(image.AsSpan(code + operand, 4));
                float changed = Changed(v);
                if (BitConverter.SingleToInt32Bits(changed) != BitConverter.SingleToInt32Bits(v))
                    Add(BitConverter.GetBytes(v), BitConverter.GetBytes(changed), $"ldc.r4 {v:R} -> {changed:R}", operand);
            }
            else if (value == 0x23)
            {
                double v = BitConverter.ToDouble(image.AsSpan(code + operand, 8));
                double changed = Changed(v);
                // NaN and infinities have no neighbour that changes the bytes; such a mutant could never be killed.
                if (BitConverter.DoubleToInt64Bits(changed) != BitConverter.DoubleToInt64Bits(v))
                    Add(BitConverter.GetBytes(v), BitConverter.GetBytes(changed), $"ldc.r8 {v:R} -> {changed:R}", operand);
            }
        }
        return found;
    }

    /// <summary>
    /// Framework collection types whose <c>.ctor(int32)</c> argument is only an initial capacity. Not observable unless
    /// the method reads capacity, so exempt by policy. Hashtable is absent: its bucket count changes enumeration order.
    /// </summary>
    private static readonly HashSet<string> CapacityTypes =
    [
        "System.Text.StringBuilder", "System.Collections.ArrayList", "System.Collections.Stack", "System.Collections.Queue",
        "System.Collections.Generic.List`1", "System.Collections.Generic.Stack`1", "System.Collections.Generic.Queue`1",
        "System.Collections.Generic.HashSet`1", "System.Collections.Generic.Dictionary`2",
    ];

    private static readonly HashSet<string> FrameworkAssemblies =
        ["System.Runtime", "System.Collections", "System.Collections.NonGeneric", "System.Private.CoreLib", "mscorlib", "netstandard"];

    /// <summary>Members through which a method can observe a collection's capacity.</summary>
    private static readonly HashSet<string> CapacityReaders = ["get_Capacity", "set_Capacity", "EnsureCapacity", "GetChunks", "TrimExcess"];

    /// <summary>The value an ldc.i4 form (ldc.i4.m1 .. ldc.i4) loads; its inline operand starts at <paramref name="operand"/>.</summary>
    private static int Constant(ushort value, System.Collections.Immutable.ImmutableArray<byte> image, int operand) => value switch
    {
        0x1F => (sbyte)image[operand],
        0x20 => BitConverter.ToInt32(image.AsSpan(operand, 4)),
        _ => value - 0x16,
    };

    /// <summary>True when the method body calls or takes the address of a <see cref="CapacityReaders"/> member.</summary>
    private static bool ReadsCapacity(BlobReader il, MetadataReader md)
    {
        while (il.RemainingBytes > 0)
        {
            byte first = il.ReadByte();
            var op = StructureReader.Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            if (op.OperandType != OperandType.InlineMethod) { Skip(ref il, op); continue; }
            var handle = MetadataTokens.EntityHandle(il.ReadInt32());
            if (handle.Kind == HandleKind.MethodSpecification)
                handle = md.GetMethodSpecification((MethodSpecificationHandle)handle).Method;
            var name = handle.Kind switch
            {
                HandleKind.MethodDefinition => md.GetMethodDefinition((MethodDefinitionHandle)handle).Name,
                HandleKind.MemberReference => md.GetMemberReference((MemberReferenceHandle)handle).Name,
                _ => default,
            };
            if (!name.IsNil && CapacityReaders.Contains(md.GetString(name))) return true;
        }
        return false;
    }

    /// <summary>
    /// True when the next instruction is <c>newobj</c> of a <see cref="CapacityTypes"/> constructor, referenced from a
    /// framework assembly, taking a single int32: the constant just loaded is a capacity hint.
    /// </summary>
    private static bool FeedsCapacity(BlobReader next, MetadataReader md)
    {
        if (next.RemainingBytes < 5 || next.ReadByte() != 0x73) return false;
        var handle = MetadataTokens.EntityHandle(next.ReadInt32());
        if (handle.Kind != HandleKind.MemberReference) return false;
        var ctor = md.GetMemberReference((MemberReferenceHandle)handle);
        if (!md.StringComparer.Equals(ctor.Name, ".ctor")) return false;
        var sig = md.GetBlobReader(ctor.Signature);
        if (!sig.ReadSignatureHeader().IsInstance || sig.ReadCompressedInteger() != 1
            || sig.ReadSignatureTypeCode() != SignatureTypeCode.Void || sig.ReadSignatureTypeCode() != SignatureTypeCode.Int32)
            return false;
        var owner = ctor.Parent;
        if (owner.Kind == HandleKind.TypeSpecification)
        {
            // A generic instance: GENERICINST (CLASS | VALUETYPE) TypeDefOrRef ...
            var spec = md.GetBlobReader(md.GetTypeSpecification((TypeSpecificationHandle)owner).Signature);
            if (spec.ReadSignatureTypeCode() != SignatureTypeCode.GenericTypeInstance) return false;
            spec.ReadCompressedInteger();
            owner = spec.ReadTypeHandle();
        }
        if (owner.Kind != HandleKind.TypeReference) return false;
        var type = md.GetTypeReference((TypeReferenceHandle)owner);
        return type.ResolutionScope.Kind == HandleKind.AssemblyReference
            && FrameworkAssemblies.Contains(md.GetString(md.GetAssemblyReference((AssemblyReferenceHandle)type.ResolutionScope).Name))
            && CapacityTypes.Contains($"{md.GetString(type.Namespace)}.{md.GetString(type.Name)}");
    }

    /// <summary>v + 1, or v * 2 where + 1 is lost to rounding; 0 becomes 1. NaN and infinities stay unchanged.</summary>
    private static double Changed(double v) => v == 0 ? 1 : v + 1 != v ? v + 1 : v * 2;
    private static float Changed(float v) => v == 0 ? 1 : v + 1 != v ? v + 1 : v * 2;

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
