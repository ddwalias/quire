using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Quire.Tools.MirrorGen;

/// <summary>
/// The logic-bearing structure of one method body: what it calls, which fields it touches, which types it tests or
/// creates, its constants, exception regions and branching, which ignore locals, nops, stack shuffling and branch
/// encoding. <see cref="Code"/> is the whole instruction sequence as encoded, with local types, initialization and max
/// stack, for <see cref="Exact"/>; <see cref="Offsets"/> the IL offset of each of its instructions (the exception
/// regions that follow them have none).
/// </summary>
sealed record Structure(string[] Calls, string[] Fields, string[] Types, string[] Constants, string[] Handlers,
    int Branches, int SwitchTargets, int Throws, int Rethrows, string[] Code, int[] Offsets)
{
    /// <summary>Whether the instruction sequences are equal, branch targets and
    /// exception regions as instruction indices: unlike <see cref="Compare"/>, branch direction and block order count,
    /// so <c>if (c) return a; return b;</c> and <c>c ? a : b</c> differ.</summary>
    public bool Exact(Structure original) => Code.SequenceEqual(original.Code);

    /// <summary>Differences from <paramref name="original"/>, one line each; empty when the structures match.</summary>
    public List<string> Compare(Structure original)
    {
        var differences = new List<string>();
        void Sequence(string what, string[] quire, string[] mathcad)
        {
            if (quire.SequenceEqual(mathcad)) return;
            int at = 0;
            while (at < quire.Length && at < mathcad.Length && quire[at] == mathcad[at]) at++;
            differences.Add($"{what} differ at #{at}: Quire {(at < quire.Length ? quire[at] : "(end)")}, " +
                $"Mathcad {(at < mathcad.Length ? mathcad[at] : "(end)")}");
        }
        void Count(string what, int quire, int mathcad)
        {
            if (quire != mathcad) differences.Add($"{what}: Quire {quire}, Mathcad {mathcad}");
        }
        Sequence("calls", Calls, original.Calls);
        Sequence("field accesses", Fields, original.Fields);
        Sequence("type operations", Types, original.Types);
        Sequence("constants", Constants, original.Constants);
        Sequence("exception regions", Handlers, original.Handlers);
        Count("conditional branches", Branches, original.Branches);
        Count("switch targets", SwitchTargets, original.SwitchTargets);
        Count("throws", Throws, original.Throws);
        Count("rethrows", Rethrows, original.Rethrows);
        return differences;
    }

    /// <summary>The first entry of <see cref="Code"/> that differs from <paramref name="original"/>'s, both sides on one
    /// line (an instruction, an exception region, or the max stack, local initialization or locals that follow them);
    /// null when <see cref="Exact"/>. It is what a failure reason quotes; <see cref="Diff"/> gives the context.</summary>
    public string? FirstDifference(Structure original)
    {
        if (Exact(original)) return null;
        int at = 0;
        while (at < Code.Length && at < original.Code.Length && Code[at] == original.Code[at]) at++;
        static string Side(string[] code, int i) => i < code.Length ? code[i] : "(end)";
        return $"first difference at #{at} (Quire has {Code.Length} entries, Mathcad {original.Code.Length}): " +
            $"Quire {Side(Code, at)}, Mathcad {Side(original.Code, at)}";
    }

    /// <summary>Where <see cref="Code"/> first differs from <paramref name="original"/>'s: that instruction and
    /// <paramref name="context"/> on either side, Quire's then Mathcad's, each Quire instruction with the source line
    /// <paramref name="where"/> gives its IL offset. Empty when <see cref="Exact"/>.</summary>
    public List<string> Diff(Structure original, int context, Func<int, string?> where)
    {
        if (Exact(original)) return [];
        int at = 0;
        while (at < Code.Length && at < original.Code.Length && Code[at] == original.Code[at]) at++;
        int from = Math.Max(0, at - context), to = Math.Min(Math.Max(Code.Length, original.Code.Length) + 1, at + context + 1);
        static string Side(string[] code, int i) => i < code.Length ? code[i] : i == code.Length ? "(end)" : "";
        int width = Enumerable.Range(from, to - from).Max(i => Side(Code, i).Length);
        var lines = new List<string> { $"first difference at instruction #{at} (Quire | Mathcad  [Quire source line])" };
        for (int i = from; i < to; i++)
            lines.Add($"{(i == at ? ">" : " ")} #{i,-4} {Side(Code, i).PadRight(width)} | {Side(original.Code, i)}"
                + (i < Offsets.Length && where(Offsets[i]) is { } line ? $"  [{line}]" : ""));
        return lines;
    }
}

/// <summary>Reads <see cref="Structure"/> from method bodies, naming every referenced member as Mathcad would
/// (<paramref name="methodName"/>: a method definition's Mathcad name, null to keep its own), compiler-generated
/// names included.
/// <paramref name="root"/>: the checkout the assembly was built from; <see cref="Body"/> hashes string literals with
/// it replaced by <c>&lt;repo&gt;</c>, since <c>[CallerFilePath]</c> (every <c>MirrorStub.NotImplemented()</c>)
/// compiles the absolute source path into the IL and the gate builds in a new clone each time.
/// <paramref name="bind"/> and <paramref name="products"/> qualify type names with their assembly for the strict
/// comparison: <paramref name="bind"/> maps (the assembly a type resolves to, its name) to Mathcad's [name, assembly], null
/// when unbound; an unbound type keeps its own assembly when
/// <paramref name="products"/> (lower-case Quire and Mathcad assembly names) lists it. Framework types stay unqualified:
/// Mathcad and Quire name them in different reference assemblies (mscorlib, System.Runtime, System.Collections...).
/// Without either, types keep their own names. A <see cref="StrictSigProvider"/> decoder also renders what
/// <see cref="SigProvider"/> erases.</summary>
sealed class StructureReader(MetadataReader md, System.Reflection.PortableExecutable.PEReader pe, ISignatureTypeProvider<TypeSig, object?> decoder,
    Func<int, string?> methodName, string? root = null,
    Func<string, string, string[]?>? bind = null, IReadOnlySet<string>? products = null)
{
    private readonly bool strict = decoder is StrictSigProvider;
    private readonly string? root = root is null ? null : Path.GetFullPath(root);
    private readonly string? self = md.IsAssembly ? md.GetString(md.GetAssemblyDefinition().Name) : null;

    internal static readonly Dictionary<ushort, OpCode> Codes = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null)!)
        .ToDictionary(o => unchecked((ushort)o.Value));

    public Structure Read(MethodDefinitionHandle handle)
    {
        var body = pe.GetMethodBody(md.GetMethodDefinition(handle).RelativeVirtualAddress);
        var il = body.GetILReader();
        List<string> calls = [], fields = [], types = [], constants = [], sequence = [];
        List<int> starts = [];
        var jumps = new Dictionary<int, int[]>();  // instruction index -> branch target offsets
        int branches = 0, switchTargets = 0, throws = 0, rethrows = 0;
        while (il.RemainingBytes > 0)
        {
            starts.Add(il.Offset);
            byte first = il.ReadByte();
            var code = Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            string operand = "";
            switch (code.OperandType)
            {
                case OperandType.InlineNone:
                    if (code == OpCodes.Throw) throws++;
                    else if (code == OpCodes.Rethrow) rethrows++;
                    else if (code.Name!.StartsWith("ldc.i4.") && code != OpCodes.Ldc_I4_0 && code != OpCodes.Ldc_I4_1)
                        constants.Add("i4 " + (code == OpCodes.Ldc_I4_M1 ? "-1" : code.Name[^1..]));
                    break;
                case OperandType.ShortInlineBrTarget:
                    int near = il.ReadSByte();
                    jumps[starts.Count - 1] = [il.Offset + near];
                    if (code.FlowControl == FlowControl.Cond_Branch) branches++;
                    break;
                case OperandType.InlineBrTarget:
                    int far = il.ReadInt32();
                    jumps[starts.Count - 1] = [il.Offset + far];
                    if (code.FlowControl == FlowControl.Cond_Branch) branches++;
                    break;
                case OperandType.InlineSwitch:
                    int[] targets = new int[il.ReadInt32()];
                    for (int i = 0; i < targets.Length; i++) targets[i] = il.ReadInt32();
                    jumps[starts.Count - 1] = [.. targets.Select(t => il.Offset + t)];
                    switchTargets += targets.Length;
                    break;
                case OperandType.ShortInlineI:
                    int small = code == OpCodes.Ldc_I4_S ? il.ReadSByte() : il.ReadByte();
                    if (code == OpCodes.Ldc_I4_S && small is not (0 or 1)) constants.Add($"i4 {small}");
                    operand = $"{small}";
                    break;
                case OperandType.InlineI:
                    int value = il.ReadInt32();
                    if (value is not (0 or 1)) constants.Add($"i4 {value}");
                    operand = $"{value}";
                    break;
                case OperandType.InlineI8: constants.Add(operand = $"i8 {il.ReadInt64()}"); break;
                case OperandType.ShortInlineR:
                    float single = il.ReadSingle();
                    constants.Add($"r8 {(double)single:R}");
                    operand = $"r4 0x{BitConverter.SingleToInt32Bits(single):x8}";
                    break;
                case OperandType.InlineR:
                    double number = il.ReadDouble();
                    constants.Add($"r8 {number:R}");
                    operand = $"r8 0x{BitConverter.DoubleToInt64Bits(number):x16}";
                    break;
                case OperandType.InlineString:
                    constants.Add(operand = "str " + OwnComponent(self, md.GetUserString(MetadataTokens.UserStringHandle(il.ReadInt32()))));
                    break;
                case OperandType.InlineMethod:
                    calls.Add($"{code.Name} {operand = Method(MetadataTokens.EntityHandle(il.ReadInt32()))}");
                    break;
                case OperandType.InlineField:
                    var fieldToken = MetadataTokens.EntityHandle(il.ReadInt32());
                    operand = $"{Field(fieldToken)} : {Render(FieldType(fieldToken))}{InitialData(fieldToken)}";
                    fields.Add($"{code.Name} {operand}");
                    break;
                case OperandType.InlineType:
                case OperandType.InlineTok:
                    var token = MetadataTokens.EntityHandle(il.ReadInt32());
                    if (code == OpCodes.Ldtoken && IsField(token))
                    {
                        operand = $"{Field(token)} : {Render(FieldType(token))}{InitialData(token)}";
                        fields.Add($"ldtoken {operand}");
                    }
                    else if (code == OpCodes.Ldtoken && token.Kind is HandleKind.MethodDefinition or HandleKind.MemberReference or HandleKind.MethodSpecification)
                        calls.Add($"ldtoken {operand = Method(token)}");
                    else if (code != OpCodes.Constrained && code != OpCodes.Ldobj && code != OpCodes.Stobj
                             && code != OpCodes.Initobj && code != OpCodes.Ldelema)
                        types.Add($"{code.Name} {operand = Type(token)}");
                    else operand = Type(token);
                    break;
                case OperandType.InlineSig:
                    operand = Signature("calli", md.GetStandaloneSignature((StandaloneSignatureHandle)MetadataTokens.EntityHandle(il.ReadInt32()))
                        .DecodeMethodSignature(decoder, null));
                    calls.Add("calli");
                    break;
                case OperandType.ShortInlineVar: operand = $"{il.ReadByte()}"; break;
                case OperandType.InlineVar: operand = $"{il.ReadUInt16()}"; break;
                default: throw new BadImageFormatException($"Unknown operand type {code.OperandType}");
            }
            sequence.Add(code.Name + (operand.Length == 0 ? "" : " " + operand));
        }
        constants.Sort(StringComparer.Ordinal);
        // Branch targets and exception regions as instruction indices: relative for branches, so block order counts.
        var index = starts.Select((offset, i) => (offset, i)).ToDictionary(p => p.offset, p => p.i);
        index[il.Offset] = starts.Count;
        string At(int offset) => index.TryGetValue(offset, out int i) ? $"{i}" : "?";
        foreach (var (at, targets) in jumps)
            sequence[at] += " " + string.Join(",", targets.Select(t => index.TryGetValue(t, out int i) ? $"{i - at}" : "?"));
        var regions = body.ExceptionRegions.OrderBy(r => r.TryOffset).ThenByDescending(r => r.TryLength).ToList();
        sequence.AddRange(regions.Select(r => $".try {At(r.TryOffset)}-{At(r.TryOffset + r.TryLength)} " +
            $"{r.Kind} {(r.Kind == ExceptionRegionKind.Filter ? At(r.FilterOffset) + " " : "")}{At(r.HandlerOffset)}-{At(r.HandlerOffset + r.HandlerLength)}"));
        sequence.Add($".maxstack {body.MaxStack}");
        sequence.Add($".initlocals {body.LocalVariablesInitialized}");
        sequence.Add(".locals " + (HasLocals(body.LocalSignature) ? string.Join(",", md.GetStandaloneSignature(body.LocalSignature)
            .DecodeLocalSignature(decoder, null).Select(Render)) : ""));
        var handlers = regions
            .Select(r => r.Kind == ExceptionRegionKind.Catch ? "catch " + Type(r.CatchType) : r.Kind.ToString().ToLowerInvariant())
            .ToArray();
        return new([.. calls], [.. fields], [.. types], [.. constants], handlers, branches, switchTargets, throws, rethrows, [.. sequence], [.. starts]);
    }

    /// <summary>What <see cref="Body"/> reports of one method body.</summary>
    public sealed record BodyFacts(string Hash, bool Stub = false);

    // A local signature that declares no locals (`.locals init ()`, which Mathcad's TlbImp-era bodies carry) is the
    // same as none; DecodeLocalSignature rejects it ("Signature type sequence must have at least one element").
    bool HasLocals(StandaloneSignatureHandle handle)
    {
        if (handle.IsNil) return false;
        var reader = md.GetBlobReader(md.GetStandaloneSignature(handle).Signature);
        reader.ReadByte(); // LOCAL_SIG
        return reader.ReadCompressedInteger() > 0;
    }

    /// <summary>
    /// A SHA-256 of everything in the body that decides what it does, independent of metadata token numbering: each
    /// instruction with its operand (tokens resolved to the member, type or string they denote; fields with their type,
    /// and for fields with initial data (<c>&lt;PrivateImplementationDetails&gt;</c> arrays and spans) a hash of those
    /// bytes), the locals' types, exception regions and the method's flags. The Release checker uses this hash to
    /// identify changed bodies; <see cref="BodyFacts.Stub"/> marks a generated MirrorStub body.
    /// </summary>
    public BodyFacts Body(MethodDefinitionHandle handle)
    {
        var definition = md.GetMethodDefinition(handle);
        var body = pe.GetMethodBody(definition.RelativeVirtualAddress);
        var il = body.GetILReader();
        var text = new System.Text.StringBuilder($"{definition.Attributes} {definition.ImplAttributes} {body.LocalVariablesInitialized}\n");
        if (HasLocals(body.LocalSignature))
            text.Append("locals ").Append(Ordinalless(string.Join(",", md.GetStandaloneSignature(body.LocalSignature)
                .DecodeLocalSignature(decoder, null).Select(Render)))).Append('\n');
        foreach (var r in body.ExceptionRegions)
            text.Append($"{r.Kind} {r.TryOffset} {r.TryLength} {r.HandlerOffset} {r.HandlerLength} {r.FilterOffset} "
                + (r.Kind == ExceptionRegionKind.Catch ? Ordinalless(Type(r.CatchType)) : "") + "\n");
        var shape = new List<string>();  // opcodes, and the callee of calls, to recognise a MirrorStub body
        while (il.RemainingBytes > 0)
        {
            text.Append(il.Offset).Append(' ');
            byte first = il.ReadByte();
            var code = Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            if (code != OpCodes.Nop) shape.Add(code.Name!);
            text.Append(code.Name).Append(' ');
            switch (code.OperandType)
            {
                case OperandType.InlineNone: break;
                case OperandType.ShortInlineBrTarget: text.Append(il.ReadSByte()); break;
                case OperandType.ShortInlineI: text.Append(code == OpCodes.Ldc_I4_S ? il.ReadSByte() : il.ReadByte()); break;
                case OperandType.ShortInlineVar: text.Append(il.ReadByte()); break;
                case OperandType.InlineVar: text.Append(il.ReadUInt16()); break;
                case OperandType.InlineBrTarget or OperandType.InlineI: text.Append(il.ReadInt32()); break;
                case OperandType.InlineI8: text.Append(il.ReadInt64()); break;
                case OperandType.ShortInlineR: text.Append(BitConverter.SingleToInt32Bits(il.ReadSingle())); break;
                case OperandType.InlineR: text.Append(BitConverter.DoubleToInt64Bits(il.ReadDouble())); break;
                case OperandType.InlineSwitch:
                    int targets = il.ReadInt32();
                    for (int i = 0; i < targets; i++) text.Append(il.ReadInt32()).Append(',');
                    break;
                case OperandType.InlineString:
                    text.Append(System.Text.Json.JsonSerializer.Serialize(Unrooted(root, md.GetUserString(MetadataTokens.UserStringHandle(il.ReadInt32())))));
                    break;
                case OperandType.InlineSig:
                    var signature = md.GetStandaloneSignature((StandaloneSignatureHandle)MetadataTokens.EntityHandle(il.ReadInt32()))
                        .DecodeMethodSignature(decoder, null);
                    text.Append(Ordinalless(Signature("calli", signature)));
                    break;
                case OperandType.InlineMethod or OperandType.InlineField or OperandType.InlineType or OperandType.InlineTok:
                    var token = MetadataTokens.EntityHandle(il.ReadInt32());
                    if (IsField(token))
                        text.Append(Ordinalless($"{Field(token)} {Render(FieldType(token))}{InitialData(token)}"));
                    else if (token.Kind is HandleKind.MethodDefinition or HandleKind.MemberReference or HandleKind.MethodSpecification)
                    {
                        string callee = Method(token);
                        text.Append(Ordinalless(callee));
                        if (code == OpCodes.Call) shape[^1] = "call " + callee;
                    }
                    else text.Append(Ordinalless(Type(token)));
                    break;
                default: throw new BadImageFormatException($"Unknown operand type {code.OperandType}");
            }
            text.Append('\n');
        }
        // Ordinals are dropped from each name as it is appended, not from string literals: their text is the body's.
        string hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(text.ToString())));
        // Generated stubs throw MirrorStub.NotImplemented(file, member), the file from [CallerFilePath].
        bool stub = shape.Count == 4 && shape[0] == "ldstr" && shape[1] == "ldstr" && shape[3] == "throw"
            && shape[2].StartsWith("call ", StringComparison.Ordinal) && shape[2].Contains("Quire.Mirror.MirrorStub::NotImplemented", StringComparison.Ordinal);
        return new(hash.ToLowerInvariant(), stub);
    }

    /// <summary>
    /// Text with the member ordinals of compiler-generated names replaced by <c>#</c> (<c>&lt;&gt;c__DisplayClass12_0</c>,
    /// <c>&lt;M&gt;b__12_3</c>, <c>&lt;&gt;9__12_3</c>, <c>&lt;M&gt;g__Local|12_0</c>, <c>&lt;M&gt;d__12</c>): declaring or
    /// removing another member renumbers them without changing what the body does. Exactness against Mathcad is still
    /// compared on the real names.
    /// </summary>
    internal static string Ordinalless(string text) => System.Text.RegularExpressions.Regex.Replace(text,
        @"(c__DisplayClass|>b__|>9__|\|)\d+(?=_\d)|(>d__)\d+", m => m.Groups[1].Success ? m.Groups[1].Value + "#" : m.Groups[2].Value + "#");

    /// <summary>A string literal with the checkout's path <paramref name="root"/> (either separator style, any
    /// trailing separator ignored) replaced by <c>&lt;repo&gt;</c> where it is a whole path prefix (followed by a
    /// separator or the end), so identical sources hash alike wherever they were built.</summary>
    internal static string Unrooted(string? root, string literal)
    {
        string trimmed = (root ?? "").TrimEnd('/', '\\');
        if (trimmed.Length == 0 || trimmed.EndsWith(':')) return literal;  // a filesystem root names every path
        foreach (string form in new[] { trimmed.Replace('\\', '/'), trimmed.Replace('/', '\\') }.Distinct())
            literal = System.Text.RegularExpressions.Regex.Replace(literal,
                System.Text.RegularExpressions.Regex.Escape(form) + @"(?=[/\\]|$)", "<repo>");
        return literal;
    }

    /// <summary>A string literal that starts with a WPF pack URI naming <paramref name="assembly"/>, the assembly that
    /// holds it (<c>/Ptc.Calculation.Core;component/…</c>, optionally <c>pack://application:,,,</c>-prefixed, with a
    /// version or key), with that name replaced by <c>&lt;self&gt;</c> (D21). The XAML compiler writes the building
    /// assembly's name there, which Quire's own identity changes; the rest of the literal still has to match. A URI
    /// naming another assembly is left as it is.</summary>
    internal static string OwnComponent(string? assembly, string literal) => assembly is null ? literal :
        System.Text.RegularExpressions.Regex.Replace(literal,
            @"^((?:pack://application:,,,)?/)" + System.Text.RegularExpressions.Regex.Escape(assembly) + @"(?=(?:;v[0-9.]+)?(?:;[0-9a-fA-F]{16})?;component/)",
            "$1<self>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    private bool IsField(EntityHandle handle) => handle.Kind == HandleKind.FieldDefinition
        || handle.Kind == HandleKind.MemberReference
        && md.GetMemberReference((MemberReferenceHandle)handle).GetKind() == MemberReferenceKind.Field;

    private TypeSig FieldType(EntityHandle handle) => handle.Kind == HandleKind.FieldDefinition
        ? md.GetFieldDefinition((FieldDefinitionHandle)handle).DecodeSignature(decoder, null)
        : md.GetMemberReference((MemberReferenceHandle)handle).DecodeFieldSignature(decoder, null);

    /// <summary>For a field of this module with initial data (an RVA field), the SHA-256 of that data, whose size its
    /// type gives (a primitive, or a value type's class layout); empty for other fields. A size it cannot tell hashes
    /// nothing, and the field's name (a content hash for compiler-emitted data) still distinguishes it.</summary>
    private string InitialData(EntityHandle handle)
    {
        if (handle.Kind != HandleKind.FieldDefinition) return "";
        var field = md.GetFieldDefinition((FieldDefinitionHandle)handle);
        if ((field.Attributes & System.Reflection.FieldAttributes.HasFieldRVA) == 0) return "";
        var blob = md.GetBlobReader(field.Signature);
        blob.ReadSignatureHeader();
        // A modified type (C++/CLI's const native data: vtables, ThrowInfo, string literals) stays unhashed: its bytes
        // hold native addresses and RVAs, which differ with the native layout whatever the source.
        // shortcut: const string literals go unchecked too; hash them once relocated words can be told apart.
        var code = blob.ReadSignatureTypeCode();
        int size = code switch
        {
            SignatureTypeCode.Boolean or SignatureTypeCode.SByte or SignatureTypeCode.Byte => 1,
            SignatureTypeCode.Char or SignatureTypeCode.Int16 or SignatureTypeCode.UInt16 => 2,
            SignatureTypeCode.Int32 or SignatureTypeCode.UInt32 or SignatureTypeCode.Single => 4,
            SignatureTypeCode.Int64 or SignatureTypeCode.UInt64 or SignatureTypeCode.Double => 8,
            SignatureTypeCode.TypeHandle when blob.ReadTypeHandle() is { Kind: HandleKind.TypeDefinition } type
                => md.GetTypeDefinition((TypeDefinitionHandle)type).GetLayout().Size,
            _ => 0,
        };
        int rva = field.GetRelativeVirtualAddress();
        var section = pe.PEHeaders.SectionHeaders.FirstOrDefault(s => rva >= s.VirtualAddress && rva < s.VirtualAddress + s.VirtualSize);
        if (size <= 0 || section.VirtualSize == 0 || rva + size > section.VirtualAddress + section.VirtualSize) return " data ?";
        // C++/CLI places zero-initialized data past the section's raw data, which the loader fills with zeros.
        var block = pe.GetSectionData(rva);
        var data = new byte[size];
        block.GetContent(0, Math.Min(size, block.Length)).CopyTo(data, 0);
        return " data " + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(data));
    }

    private string Method(EntityHandle handle)
    {
        switch (handle.Kind)
        {
            case HandleKind.MethodDefinition:
                var definition = md.GetMethodDefinition((MethodDefinitionHandle)handle);
                var sig = definition.DecodeSignature(decoder, null);
                string name = methodName(MetadataTokens.GetToken(handle)) ?? md.GetString(definition.Name);
                return $"{Type(definition.GetDeclaringType())}::{Signature(name, sig)}";
            case HandleKind.MemberReference:
                var reference = md.GetMemberReference((MemberReferenceHandle)handle);
                return $"{Owner(reference.Parent)}::{Signature(md.GetString(reference.Name), reference.DecodeMethodSignature(decoder, null))}";
            case HandleKind.MethodSpecification:
                var specification = md.GetMethodSpecification((MethodSpecificationHandle)handle);
                var args = specification.DecodeSignature(decoder, null);
                return $"{Method(specification.Method)}<{string.Join(",", args.Select(Render))}>";
            default:
                throw new BadImageFormatException($"Unexpected method token kind {handle.Kind}");
        }
    }

    private string Field(EntityHandle handle)
    {
        if (handle.Kind == HandleKind.FieldDefinition)
        {
            var field = md.GetFieldDefinition((FieldDefinitionHandle)handle);
            return $"{Type(field.GetDeclaringType())}::{md.GetString(field.Name)}";
        }
        var reference = md.GetMemberReference((MemberReferenceHandle)handle);
        return $"{Owner(reference.Parent)}::{md.GetString(reference.Name)}";
    }

    private string Owner(EntityHandle parent) => parent.Kind switch
    {
        HandleKind.TypeDefinition or HandleKind.TypeReference or HandleKind.TypeSpecification => Type(parent),
        HandleKind.MethodDefinition => Method(parent),
        _ => "<module>",
    };

    private string Type(EntityHandle handle) => handle.Kind switch
    {
        // The decoder names the assembly a definition or reference resolves to, which Name qualifies with.
        HandleKind.TypeDefinition => Render(decoder.GetTypeFromDefinition(md, (TypeDefinitionHandle)handle, 0)),
        HandleKind.TypeReference => Render(decoder.GetTypeFromReference(md, (TypeReferenceHandle)handle, 0)),
        HandleKind.TypeSpecification => Render(md.GetTypeSpecification((TypeSpecificationHandle)handle).DecodeSignature(decoder, null)),
        _ => throw new BadImageFormatException($"Unexpected type token kind {handle.Kind}"),
    };

    /// <summary>Strict: with the calling convention and header flags (instance, explicit this) and the vararg sentinel.</summary>
    private string Signature(string name, MethodSignature<TypeSig> sig) =>
        $"{(strict ? Header(sig.Header) : "")}{name}`{sig.GenericParameterCount}({Parameters(sig)})->{Render(sig.ReturnType)}";

    private string Parameters(MethodSignature<TypeSig> sig) => string.Join(",", sig.ParameterTypes.Select(Render)
        .Select((p, i) => strict && i == sig.RequiredParameterCount ? "...," + p : p));

    private static string Header(SignatureHeader header) => (header.IsInstance ? "instance " : "")
        + (header.HasExplicitThis ? "explicit " : "")
        + (header.CallingConvention == SignatureCallingConvention.Default ? "" : header.CallingConvention.ToString().ToLowerInvariant() + " ");

    private string Render(TypeSig sig) => sig switch
    {
        NamedSig n => Name(n.Name, n.Assembly) + (n.Args.IsEmpty ? "" : "<" + string.Join(",", n.Args.Select(Render)) + ">"),
        ArraySig a => Render(a.Element) + "[" + new string(',', Math.Max(0, a.Rank - 1)) + "]",
        ByRefSig b => Render(b.Element) + "&",
        PointerSig p => Render(p.Element) + "*",
        GenericParamSig g => (g.OfMethod ? "!!" : "!") + g.Index,
        ModifiedSig m => $"{Render(m.Element)} {(m.Required ? "modreq" : "modopt")}({Render(m.Modifier)})",
        PinnedSig p => Render(p.Element) + " pinned",
        MethodPointerSig f => $"method {Header(f.Signature.Header)}{Render(f.Signature.ReturnType)}*({Parameters(f.Signature)})",
        // ilasm's notation: lower...upper, lower..., or a size; an unspecified dimension is empty.
        ShapedArraySig s => Render(s.Element) + "[" + string.Join(",", Enumerable.Range(0, s.Shape.Rank).Select(i =>
            (i < s.Shape.LowerBounds.Length ? (int?)s.Shape.LowerBounds[i] : null, i < s.Shape.Sizes.Length ? (int?)s.Shape.Sizes[i] : null) switch
            {
                ({ } low, { } size) => $"{low}...{low + size - 1}",
                ({ } low, null) => $"{low}...",
                (null, { } size) => $"{size}",
                _ => "",
            })) + "]",
        _ => sig.ToString(),
    };

    /// <summary>Mathcad's name for a type: mirrored types map whole, else their outermost mirrored declaring type;
    /// with <c>bind</c>, prefixed by its lower-case assembly unless that is a framework one.</summary>
    private string Name(string type, string assembly)
    {
        if (bind == null && products == null) return type;
        // A module's own compiler types (<Module>, <PrivateImplementationDetails>...) are its own on either side.
        if (type.StartsWith('<') && assembly == self) return "[<self>]" + type;
        int nest = type.IndexOf('+');
        var bound = bind?.Invoke(assembly, type)
            ?? (nest > 0 && bind?.Invoke(assembly, type[..nest]) is [var outer, var at] ? [outer + type[nest..], at] : null);
        string name = bound?[0] ?? type, scope = (bound?[1] ?? assembly).ToLowerInvariant();
        return (bound != null || products?.Contains(scope) == true ? $"[{scope}]" : "") + name;
    }
}

/// <summary>What <see cref="SigProvider"/> (shared with MirrorGen, whose generated code must not change) erases, kept for
/// the strict comparison: custom modifiers (C++/CLI's IsLong, IsConst, IsVolatile; C#'s <c>in</c>), pinned locals,
/// function pointer signatures and array bounds.</summary>
sealed record ModifiedSig(TypeSig Modifier, TypeSig Element, bool Required) : TypeSig;
sealed record PinnedSig(TypeSig Element) : TypeSig;
sealed record MethodPointerSig(MethodSignature<TypeSig> Signature) : TypeSig;
sealed record ShapedArraySig(TypeSig Element, ArrayShape Shape) : TypeSig;

/// <summary><see cref="SigProvider"/>'s names, with nothing erased (see <see cref="ModifiedSig"/>).</summary>
sealed class StrictSigProvider(string assembly) : ISignatureTypeProvider<TypeSig, object?>
{
    private readonly SigProvider names = new(assembly);
    public TypeSig GetPrimitiveType(PrimitiveTypeCode typeCode) => names.GetPrimitiveType(typeCode);
    public TypeSig GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) =>
        names.GetTypeFromDefinition(reader, handle, rawTypeKind);
    public TypeSig GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) =>
        names.GetTypeFromReference(reader, handle, rawTypeKind);
    public TypeSig GetTypeFromSpecification(MetadataReader reader, object? context, TypeSpecificationHandle handle, byte rawTypeKind) =>
        reader.GetTypeSpecification(handle).DecodeSignature(this, context);
    public TypeSig GetGenericInstantiation(TypeSig genericType, System.Collections.Immutable.ImmutableArray<TypeSig> typeArguments) =>
        names.GetGenericInstantiation(genericType, typeArguments);
    public TypeSig GetArrayType(TypeSig elementType, ArrayShape shape) => new ShapedArraySig(elementType, shape);
    public TypeSig GetSZArrayType(TypeSig elementType) => names.GetSZArrayType(elementType);
    public TypeSig GetPointerType(TypeSig elementType) => names.GetPointerType(elementType);
    public TypeSig GetByReferenceType(TypeSig elementType) => names.GetByReferenceType(elementType);
    public TypeSig GetGenericMethodParameter(object? context, int index) => names.GetGenericMethodParameter(context, index);
    public TypeSig GetGenericTypeParameter(object? context, int index) => names.GetGenericTypeParameter(context, index);
    public TypeSig GetPinnedType(TypeSig elementType) => new PinnedSig(elementType);
    public TypeSig GetFunctionPointerType(MethodSignature<TypeSig> signature) => new MethodPointerSig(signature);
    public TypeSig GetModifiedType(TypeSig modifier, TypeSig unmodifiedType, bool isRequired) => new ModifiedSig(modifier, unmodifiedType, isRequired);
}
