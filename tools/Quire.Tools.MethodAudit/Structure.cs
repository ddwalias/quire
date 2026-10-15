using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Quire.Tools.MirrorGen;

/// <summary>
/// The logic-bearing structure of one method body: what it calls, which fields it touches, which types it tests or
/// creates, its constants, exception regions and branching. Locals, nops, stack shuffling and branch encoding are
/// ignored, so a Debug reconstruction can match a Release original.
/// </summary>
sealed record Structure(string[] Calls, string[] Fields, string[] Types, string[] Constants, string[] Handlers,
    int Branches, int SwitchTargets, int Throws, int Rethrows)
{
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
}

/// <summary>Reads <see cref="Structure"/> from method bodies, naming every referenced member as Mathcad would.
/// <paramref name="exact"/> keeps compiler-generated names (for <see cref="Body"/>, which names members as declared).
/// <paramref name="root"/>: the checkout the assembly was built from; <see cref="Body"/> hashes string literals with
/// it replaced by <c>&lt;repo&gt;</c>, since <c>[CallerFilePath]</c> (every <c>MirrorStub.NotImplemented()</c>)
/// compiles the absolute source path into the IL and the gate builds in a new clone each time.</summary>
sealed class StructureReader(MetadataReader md, System.Reflection.PortableExecutable.PEReader pe, SigProvider decoder,
    Func<string, string> typeName, Func<int, string?> methodName, bool exact = false, string? root = null)
{
    private readonly string? root = root is null ? null : Path.GetFullPath(root);

    internal static readonly Dictionary<ushort, OpCode> Codes = typeof(OpCodes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null)!)
        .ToDictionary(o => unchecked((ushort)o.Value));

    public Structure Read(MethodDefinitionHandle handle)
    {
        var body = pe.GetMethodBody(md.GetMethodDefinition(handle).RelativeVirtualAddress);
        var il = body.GetILReader();
        List<string> calls = [], fields = [], types = [], constants = [];
        int branches = 0, switchTargets = 0, throws = 0, rethrows = 0;
        while (il.RemainingBytes > 0)
        {
            byte first = il.ReadByte();
            var code = Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            switch (code.OperandType)
            {
                case OperandType.InlineNone:
                    if (code == OpCodes.Throw) throws++;
                    else if (code == OpCodes.Rethrow) rethrows++;
                    else if (code.Name!.StartsWith("ldc.i4.") && code != OpCodes.Ldc_I4_0 && code != OpCodes.Ldc_I4_1)
                        constants.Add("i4 " + (code == OpCodes.Ldc_I4_M1 ? "-1" : code.Name[^1..]));
                    break;
                case OperandType.ShortInlineBrTarget:
                    il.ReadSByte();
                    if (code.FlowControl == FlowControl.Cond_Branch) branches++;
                    break;
                case OperandType.InlineBrTarget:
                    il.ReadInt32();
                    if (code.FlowControl == FlowControl.Cond_Branch) branches++;
                    break;
                case OperandType.InlineSwitch:
                    int targets = il.ReadInt32();
                    for (int i = 0; i < targets; i++) il.ReadInt32();
                    switchTargets += targets;
                    break;
                case OperandType.ShortInlineI:
                    int small = code == OpCodes.Ldc_I4_S ? il.ReadSByte() : il.ReadByte();
                    if (code == OpCodes.Ldc_I4_S && small is not (0 or 1)) constants.Add($"i4 {small}");
                    break;
                case OperandType.InlineI:
                    int value = il.ReadInt32();
                    if (value is not (0 or 1)) constants.Add($"i4 {value}");
                    break;
                case OperandType.InlineI8: constants.Add($"i8 {il.ReadInt64()}"); break;
                case OperandType.ShortInlineR: constants.Add($"r8 {(double)il.ReadSingle():R}"); break;
                case OperandType.InlineR: constants.Add($"r8 {il.ReadDouble():R}"); break;
                case OperandType.InlineString:
                    constants.Add("str " + md.GetUserString(MetadataTokens.UserStringHandle(il.ReadInt32())));
                    break;
                case OperandType.InlineMethod:
                    calls.Add($"{code.Name} {Method(MetadataTokens.EntityHandle(il.ReadInt32()))}");
                    break;
                case OperandType.InlineField:
                    fields.Add($"{code.Name} {Field(MetadataTokens.EntityHandle(il.ReadInt32()))}");
                    break;
                case OperandType.InlineType:
                case OperandType.InlineTok:
                    var token = MetadataTokens.EntityHandle(il.ReadInt32());
                    if (code == OpCodes.Ldtoken && token.Kind is HandleKind.MethodDefinition or HandleKind.MemberReference)
                        calls.Add($"ldtoken {Method(token)}");
                    else if (code != OpCodes.Constrained && code != OpCodes.Ldobj && code != OpCodes.Stobj
                             && code != OpCodes.Initobj && code != OpCodes.Ldelema)
                        types.Add($"{code.Name} {Type(token)}");
                    break;
                case OperandType.InlineSig: il.ReadInt32(); calls.Add("calli"); break;
                case OperandType.ShortInlineVar: il.ReadByte(); break;
                case OperandType.InlineVar: il.ReadUInt16(); break;
                default: throw new BadImageFormatException($"Unknown operand type {code.OperandType}");
            }
        }
        constants.Sort(StringComparer.Ordinal);
        var handlers = body.ExceptionRegions.OrderBy(r => r.TryOffset).ThenByDescending(r => r.TryLength)
            .Select(r => r.Kind == ExceptionRegionKind.Catch ? "catch " + Type(r.CatchType) : r.Kind.ToString().ToLowerInvariant())
            .ToArray();
        return new([.. calls], [.. fields], [.. types], [.. constants], handlers, branches, switchTargets, throws, rethrows);
    }

    /// <summary>What <see cref="Body"/> reports of one method body.</summary>
    public sealed record BodyFacts(string Hash, int[] Calls, string[] VirtualCalls, string[] Reads, string[] Writes,
        string[] ReferenceReads);

    /// <summary>
    /// A SHA-256 of everything in the body that decides what it does, independent of metadata token numbering: each
    /// instruction with its operand (tokens resolved to the member, type or string they denote; fields with their type,
    /// and for fields with initial data (<c>&lt;PrivateImplementationDetails&gt;</c> arrays and spans) a hash of those
    /// bytes), the locals' types, exception regions and the method's flags. Unchanged unless the method's IL changed,
    /// so the validator mutates only methods whose hash differs from the last validated build. Also what selects
    /// methods affected by another's change: the tokens of this module's methods the body references (calls,
    /// delegates, ldtoken; every same-named method of a local generic type reached through an instantiation), the
    /// dispatch slots (<see cref="SlotOf"/>) of its virtual calls, and the fields it reads and writes (taking a field's
    /// address counts as both), as <c>DeclaringType::name</c> with generic instantiations reduced to their definition;
    /// and the fields it reads that hold a mutable reference (<see cref="HoldsReference"/>), through which it may change
    /// what other readers see (an array or collection it fills) without storing to the field.
    /// </summary>
    public BodyFacts Body(MethodDefinitionHandle handle)
    {
        var definition = md.GetMethodDefinition(handle);
        var body = pe.GetMethodBody(definition.RelativeVirtualAddress);
        var il = body.GetILReader();
        var text = new System.Text.StringBuilder($"{definition.Attributes} {definition.ImplAttributes} {body.LocalVariablesInitialized}\n");
        if (!body.LocalSignature.IsNil)
            text.Append("locals ").AppendJoin(",", md.GetStandaloneSignature(body.LocalSignature)
                .DecodeLocalSignature(decoder, null).Select(Render)).Append('\n');
        foreach (var r in body.ExceptionRegions)
            text.Append($"{r.Kind} {r.TryOffset} {r.TryLength} {r.HandlerOffset} {r.HandlerLength} {r.FilterOffset} "
                + (r.Kind == ExceptionRegionKind.Catch ? Type(r.CatchType) : "") + "\n");
        var calls = new SortedSet<int>();
        var slots = new SortedSet<string>(StringComparer.Ordinal);
        var reads = new SortedSet<string>(StringComparer.Ordinal);
        var writes = new SortedSet<string>(StringComparer.Ordinal);
        var referenceReads = new SortedSet<string>(StringComparer.Ordinal);
        while (il.RemainingBytes > 0)
        {
            text.Append(il.Offset).Append(' ');
            byte first = il.ReadByte();
            var code = Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
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
                    text.Append(Signature("calli", signature));
                    break;
                case OperandType.InlineMethod or OperandType.InlineField or OperandType.InlineType or OperandType.InlineTok:
                    var token = MetadataTokens.EntityHandle(il.ReadInt32());
                    if (token.Kind is HandleKind.FieldDefinition || token.Kind is HandleKind.MemberReference
                        && md.GetMemberReference((MemberReferenceHandle)token).GetKind() == MemberReferenceKind.Field)
                    {
                        text.Append(Field(token)).Append(' ').Append(Render(FieldType(token))).Append(InitialData(token));
                        string id = FieldId(token);
                        if (code == OpCodes.Ldfld || code == OpCodes.Ldsfld || code == OpCodes.Ldflda || code == OpCodes.Ldsflda)
                        {
                            reads.Add(id);
                            if (HoldsReference(token)) referenceReads.Add(id);
                        }
                        if (code == OpCodes.Stfld || code == OpCodes.Stsfld || code == OpCodes.Ldflda || code == OpCodes.Ldsflda)
                            writes.Add(id);
                    }
                    else if (token.Kind is HandleKind.MethodDefinition or HandleKind.MemberReference or HandleKind.MethodSpecification)
                    {
                        text.Append(Method(token));
                        Callees(token, calls);
                        if (code == OpCodes.Callvirt || code == OpCodes.Ldvirtftn) slots.Add(Slot(token));
                    }
                    else text.Append(Type(token));
                    break;
                default: throw new BadImageFormatException($"Unknown operand type {code.OperandType}");
            }
            text.Append('\n');
        }
        string hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(text.ToString())));
        return new(hash.ToLowerInvariant(), [.. calls], [.. slots], [.. reads], [.. writes], [.. referenceReads]);
    }

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

    private TypeSig FieldType(EntityHandle handle) => handle.Kind == HandleKind.FieldDefinition
        ? md.GetFieldDefinition((FieldDefinitionHandle)handle).DecodeSignature(decoder, null)
        : md.GetMemberReference((MemberReferenceHandle)handle).DecodeFieldSignature(decoder, null);

    /// <summary>Whether a field's type may hold a mutable object: any reference type but <c>string</c>, and a type
    /// parameter (it may be one).</summary>
    private bool HoldsReference(EntityHandle handle)
    {
        var blob = md.GetBlobReader(handle.Kind == HandleKind.FieldDefinition
            ? md.GetFieldDefinition((FieldDefinitionHandle)handle).Signature
            : md.GetMemberReference((MemberReferenceHandle)handle).Signature);
        blob.ReadSignatureHeader();
        byte code = blob.ReadByte();
        while (code is 0x1f or 0x20) // custom modifiers
        {
            blob.ReadTypeHandle();
            code = blob.ReadByte();
        }
        return code switch
        {
            0x12 or 0x14 or 0x1c or 0x1d or 0x13 or 0x1e => true, // class, array, object, szarray, !n, !!n
            0x15 => blob.ReadByte() == 0x12,                         // generic instance of a class
            _ => false,                                              // primitives, string, value types, pointers
        };
    }

    /// <summary>The type definition a generic instantiation (<c>Foo&lt;int&gt;</c>) is made from; nil otherwise.</summary>
    private TypeDefinitionHandle GenericDefinition(EntityHandle parent)
    {
        if (parent.Kind != HandleKind.TypeSpecification) return default;
        var blob = md.GetBlobReader(md.GetTypeSpecification((TypeSpecificationHandle)parent).Signature);
        if (blob.ReadSignatureTypeCode() != SignatureTypeCode.GenericTypeInstance) return default;
        blob.ReadCompressedInteger();  // CLASS or VALUETYPE
        return blob.ReadTypeHandle() is { Kind: HandleKind.TypeDefinition } type ? (TypeDefinitionHandle)type : default;
    }

    /// <summary><c>DeclaringType::name</c> of a field operand, as declared (instantiations reduced to their
    /// definition), so readers and writers of one field match whatever instantiation each names.</summary>
    private string FieldId(EntityHandle handle)
    {
        if (handle.Kind == HandleKind.FieldDefinition)
        {
            var field = md.GetFieldDefinition((FieldDefinitionHandle)handle);
            return $"{SigProvider.FullName(md, field.GetDeclaringType())}::{md.GetString(field.Name)}";
        }
        var reference = md.GetMemberReference((MemberReferenceHandle)handle);
        var generic = GenericDefinition(reference.Parent);
        return $"{(generic.IsNil ? Owner(reference.Parent) : SigProvider.FullName(md, generic))}::{md.GetString(reference.Name)}";
    }

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
        if (size <= 0) return " data ?";
        var data = pe.GetSectionData(field.GetRelativeVirtualAddress()).GetContent(0, size);
        return " data " + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(data.AsSpan()));
    }

    /// <summary>Tokens of this module's method definitions a method operand can reach.</summary>
    private void Callees(EntityHandle handle, SortedSet<int> calls)
    {
        switch (handle.Kind)
        {
            case HandleKind.MethodDefinition: calls.Add(MetadataTokens.GetToken(handle)); break;
            case HandleKind.MethodSpecification: Callees(md.GetMethodSpecification((MethodSpecificationHandle)handle).Method, calls); break;
            case HandleKind.MemberReference:
                var reference = md.GetMemberReference((MemberReferenceHandle)handle);
                // A method of a generic type of this module, through an instantiation: its same-named methods.
                var type = GenericDefinition(reference.Parent);
                if (!type.IsNil)
                    foreach (var method in md.GetTypeDefinition(type).GetMethods())
                        if (md.StringComparer.Equals(md.GetMethodDefinition(method).Name, md.GetString(reference.Name)))
                            calls.Add(MetadataTokens.GetToken(method));
                break;
        }
    }

    /// <summary>
    /// A dispatch slot: <c>name`arity/parameter count</c>, the name unqualified (an explicit implementation
    /// <c>Ns.IFoo&lt;T&gt;.Bar</c> is <c>Bar</c>). Parameter types are left out on purpose: a call through
    /// <c>Base&lt;int&gt;::M(!0)</c> must match the override <c>M(int)</c> and <c>D&lt;T&gt;::M(!0)</c> alike, so
    /// this over-selects rather than misses. <c>validate.slot_of_key</c> derives the same text from an inventory key.
    /// </summary>
    public static string SlotOf(string name, int arity, int parameters) => $"{name.Split('.')[^1]}`{arity}/{parameters}";

    /// <summary>The slot of a method definition (for the inventory's virtual methods).</summary>
    public string Slot(MethodDefinitionHandle handle) => Slot((EntityHandle)handle);

    private string Slot(EntityHandle handle)
    {
        switch (handle.Kind)
        {
            case HandleKind.MethodSpecification:
                return Slot(md.GetMethodSpecification((MethodSpecificationHandle)handle).Method);
            case HandleKind.MethodDefinition:
                var definition = md.GetMethodDefinition((MethodDefinitionHandle)handle);
                var sig = definition.DecodeSignature(decoder, null);
                return SlotOf(md.GetString(definition.Name), sig.GenericParameterCount, sig.ParameterTypes.Length);
            default:
                var reference = md.GetMemberReference((MemberReferenceHandle)handle);
                var refSig = reference.DecodeMethodSignature(decoder, null);
                return SlotOf(md.GetString(reference.Name), refSig.GenericParameterCount, refSig.ParameterTypes.Length);
        }
    }

    private string Method(EntityHandle handle)
    {
        switch (handle.Kind)
        {
            case HandleKind.MethodDefinition:
                var definition = md.GetMethodDefinition((MethodDefinitionHandle)handle);
                var sig = definition.DecodeSignature(decoder, null);
                string name = methodName(MetadataTokens.GetToken(handle)) ?? md.GetString(definition.Name);
                return $"{Name(SigProvider.FullName(md, definition.GetDeclaringType()))}::{Signature(name, sig)}";
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
            return $"{Name(SigProvider.FullName(md, field.GetDeclaringType()))}::{Member(md.GetString(field.Name))}";
        }
        var reference = md.GetMemberReference((MemberReferenceHandle)handle);
        return $"{Owner(reference.Parent)}::{Member(md.GetString(reference.Name))}";
    }

    private string Owner(EntityHandle parent) => parent.Kind switch
    {
        HandleKind.TypeDefinition or HandleKind.TypeReference or HandleKind.TypeSpecification => Type(parent),
        HandleKind.MethodDefinition => Method(parent),
        _ => "<module>",
    };

    private string Type(EntityHandle handle) => handle.Kind switch
    {
        HandleKind.TypeDefinition => Name(SigProvider.FullName(md, (TypeDefinitionHandle)handle)),
        HandleKind.TypeReference => Name(SigProvider.FullName(md, (TypeReferenceHandle)handle)),
        HandleKind.TypeSpecification => Render(md.GetTypeSpecification((TypeSpecificationHandle)handle).DecodeSignature(decoder, null)),
        _ => throw new BadImageFormatException($"Unexpected type token kind {handle.Kind}"),
    };

    private string Signature(string name, MethodSignature<TypeSig> sig) =>
        $"{Member(name)}`{sig.GenericParameterCount}({string.Join(",", sig.ParameterTypes.Select(Render))})->{Render(sig.ReturnType)}";

    private string Render(TypeSig sig) => sig switch
    {
        NamedSig n => Name(n.Name) + (n.Args.IsEmpty ? "" : "<" + string.Join(",", n.Args.Select(Render)) + ">"),
        ArraySig a => Render(a.Element) + "[" + new string(',', Math.Max(0, a.Rank - 1)) + "]",
        ByRefSig b => Render(b.Element) + "&",
        PointerSig p => Render(p.Element) + "*",
        GenericParamSig g => (g.OfMethod ? "!!" : "!") + g.Index,
        _ => sig.ToString(),
    };

    /// <summary>Compiler-generated names (closures, iterators, backing fields) differ between compilers.</summary>
    private string Member(string name) => !exact && name.Contains('<') ? "<generated>" : name;

    /// <summary>Mathcad's name for a type: mirrored types map whole, else their outermost mirrored declaring type.</summary>
    private string Name(string type)
    {
        string mapped = typeName(type);
        int nest = type.IndexOf('+');
        if (mapped == type && nest > 0) mapped = typeName(type[..nest]) + type[nest..];
        return string.Join("+", mapped.Split('+').Select(Member));
    }
}

static class CompilerGenerated
{
    /// <summary>
    /// The hand-written method a compiler-generated one implements: a lambda or local function <c>&lt;Name&gt;b__…</c> /
    /// <c>&lt;Name&gt;g__…</c> (in the type or a <c>&lt;&gt;c…</c> closure class), or a method of an iterator or async
    /// state machine <c>&lt;Name&gt;d__…</c>. Null for shared closure plumbing such as the constructors of <c>&lt;&gt;c</c>.
    /// </summary>
    public static (string Type, string Name)? Owner(string typeName, string methodName)
    {
        var parts = typeName.Split('+');
        string type = string.Join('+', parts.TakeWhile(p => !p.StartsWith('<')));
        // The text inside the leading <...>, brackets matched by depth (`<Ns.IFoo<T>.Bar>b__0_0`), unwrapped again when
        // it is itself compiler-named (`<<Bar>g__Local|3_0>d` is the state machine of local function Local in Bar).
        static string? Inner(string name)
        {
            if (!name.StartsWith('<')) return null;
            int depth = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == '<') depth++;
                else if (name[i] == '>' && --depth == 0)
                    return i <= 1 ? null : name[1] == '<' ? Inner(name[1..i]) : name[1..i];
            }
            return null;
        }
        string? name = Inner(methodName) ?? parts.Select(Inner).LastOrDefault(n => n != null);
        return type.Length == 0 || name == null ? null : (type, name);
    }
}

/// <summary>Dependencies between the top-level types of a test assembly, read from IL: which decide what a change
/// to one test file can affect (shared helpers, including extension methods a text search cannot attribute).</summary>
static class TestTypes
{
    public static string Name(MetadataReader md, TypeDefinitionHandle handle) => SigProvider.FullName(md, handle);

    /// <summary>The top-level type declaring <paramref name="handle"/> (itself when it is top-level).</summary>
    static TypeDefinitionHandle TopLevel(MetadataReader md, TypeDefinitionHandle handle)
    {
        while (md.GetTypeDefinition(handle).GetDeclaringType() is { IsNil: false } outer) handle = outer;
        return handle;
    }

    /// <summary>The reflection name (<c>Namespace.Outer+Nested</c>) of a type definition or reference, as
    /// attribute blobs serialize types.</summary>
    static string ReflectionName(MetadataReader md, EntityHandle handle)
    {
        if (handle.Kind == HandleKind.TypeDefinition)
        {
            var type = md.GetTypeDefinition((TypeDefinitionHandle)handle);
            return type.GetDeclaringType() is { IsNil: false } outer ? ReflectionName(md, outer) + "+" + md.GetString(type.Name)
                : md.GetString(type.Namespace) is { Length: > 0 } ns ? ns + "." + md.GetString(type.Name) : md.GetString(type.Name);
        }
        if (handle.Kind == HandleKind.TypeReference)
        {
            var type = md.GetTypeReference((TypeReferenceHandle)handle);
            return type.ResolutionScope.Kind == HandleKind.TypeReference ? ReflectionName(md, type.ResolutionScope) + "+" + md.GetString(type.Name)
                : md.GetString(type.Namespace) is { Length: > 0 } ns ? ns + "." + md.GetString(type.Name) : md.GetString(type.Name);
        }
        return "?";
    }

    /// <summary>Other top-level types of this assembly that <paramref name="top"/> or its nested types derive from or
    /// implement, whose members or types their method bodies reference, or that the custom attributes on them, their
    /// methods, fields, properties and parameters construct or name in a <c>System.Type</c> argument
    /// (<c>[MemberData(..., MemberType = typeof(A))]</c>, <c>[ClassData(typeof(A.Data))]</c>, a data attribute A
    /// declares).</summary>
    public static SortedSet<TypeDefinitionHandle> References(MetadataReader md, System.Reflection.PortableExecutable.PEReader pe, TypeDefinitionHandle top)
    {
        var found = new HashSet<TypeDefinitionHandle>();
        var byName = md.TypeDefinitions.ToDictionary(h => ReflectionName(md, h));
        void Type(EntityHandle handle)
        {
            switch (handle.Kind)
            {
                case HandleKind.TypeDefinition: found.Add(TopLevel(md, (TypeDefinitionHandle)handle)); break;
                case HandleKind.TypeSpecification:
                    // A generic instantiation, array or pointer: every type definition of this assembly in it.
                    md.GetTypeSpecification((TypeSpecificationHandle)handle).DecodeSignature(new Collector(md, found), null);
                    break;
            }
        }
        void Member(EntityHandle handle)
        {
            switch (handle.Kind)
            {
                case HandleKind.MethodDefinition:
                    Type(md.GetMethodDefinition((MethodDefinitionHandle)handle).GetDeclaringType()); break;
                case HandleKind.FieldDefinition:
                    Type(md.GetFieldDefinition((FieldDefinitionHandle)handle).GetDeclaringType()); break;
                case HandleKind.MethodSpecification:
                    var specification = md.GetMethodSpecification((MethodSpecificationHandle)handle);
                    Member(specification.Method);
                    specification.DecodeSignature(new Collector(md, found), null);
                    break;
                case HandleKind.MemberReference:
                    Type(md.GetMemberReference((MemberReferenceHandle)handle).Parent); break;
                default: Type(handle); break;
            }
        }
        // Every type name a serialized System.Type (`Ns.A+Data, Assembly, Version=...`, generic arguments in brackets)
        // mentions; a name of another assembly that matches one of this assembly's types only over-approximates.
        void Named(string? serialized)
        {
            foreach (var part in (serialized ?? "").Split(['[', ']', ',', ' '], StringSplitOptions.RemoveEmptyEntries))
                if (byName.TryGetValue(part, out var handle)) found.Add(TopLevel(md, handle));
        }
        void Arguments(IEnumerable<CustomAttributeTypedArgument<string>> arguments)
        {
            foreach (var argument in arguments)
            {
                Named(argument.Type);  // an enum of this assembly
                if (argument.Value is System.Collections.Immutable.ImmutableArray<CustomAttributeTypedArgument<string>> elements) Arguments(elements);
                else if (argument.Type == AttributeTypes.SystemType) Named(argument.Value as string);
            }
        }
        void Attributes(CustomAttributeHandleCollection attributes)
        {
            foreach (var handle in attributes)
            {
                var attribute = md.GetCustomAttribute(handle);
                Member(attribute.Constructor);
                try
                {
                    var value = attribute.DecodeValue(new AttributeTypes(md, byName));
                    Arguments(value.FixedArguments);
                    Arguments(value.NamedArguments.Select(a => new CustomAttributeTypedArgument<string>(a.Type, a.Value)));
                }
                catch (Exception e) when (e is BadImageFormatException or NotSupportedException)
                {
                    // An argument of an enum this assembly does not declare: its size is unknown, so the blob cannot
                    // be decoded. Count every type of this assembly whose name the blob spells out.
                    string blob = System.Text.Encoding.UTF8.GetString(md.GetBlobBytes(attribute.Value));
                    foreach (var (name, type) in byName)
                        if (blob.Contains(name, StringComparison.Ordinal)) found.Add(TopLevel(md, type));
                }
            }
        }
        var types = new Stack<TypeDefinitionHandle>([top]);
        while (types.Count > 0)
        {
            var type = md.GetTypeDefinition(types.Pop());
            if (!type.BaseType.IsNil) Type(type.BaseType);
            foreach (var implementation in type.GetInterfaceImplementations())
                Type(md.GetInterfaceImplementation(implementation).Interface);
            Attributes(type.GetCustomAttributes());
            foreach (var field in type.GetFields()) Attributes(md.GetFieldDefinition(field).GetCustomAttributes());
            foreach (var property in type.GetProperties()) Attributes(md.GetPropertyDefinition(property).GetCustomAttributes());
            foreach (var nested in type.GetNestedTypes()) types.Push(nested);
            foreach (var handle in type.GetMethods())
            {
                var method = md.GetMethodDefinition(handle);
                Attributes(method.GetCustomAttributes());
                foreach (var parameter in method.GetParameters()) Attributes(md.GetParameter(parameter).GetCustomAttributes());
                if (method.RelativeVirtualAddress == 0) continue;
                var il = pe.GetMethodBody(method.RelativeVirtualAddress).GetILReader();
                while (il.RemainingBytes > 0)
                {
                    byte first = il.ReadByte();
                    var code = StructureReader.Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
                    switch (code.OperandType)
                    {
                        case OperandType.InlineMethod or OperandType.InlineField or OperandType.InlineType or OperandType.InlineTok:
                            Member(MetadataTokens.EntityHandle(il.ReadInt32())); break;
                        case OperandType.InlineSwitch: il.Offset += 4 * il.ReadInt32(); break;
                        case OperandType.InlineNone: break;
                        case OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar: il.Offset += 1; break;
                        case OperandType.InlineVar: il.Offset += 2; break;
                        case OperandType.InlineI8 or OperandType.InlineR: il.Offset += 8; break;
                        default: il.Offset += 4; break;
                    }
                }
            }
        }
        found.Remove(top);
        return new SortedSet<TypeDefinitionHandle>(found, Comparer<TypeDefinitionHandle>.Create(
            (a, b) => MetadataTokens.GetRowNumber(a).CompareTo(MetadataTokens.GetRowNumber(b))));
    }

    /// <summary>Attribute argument types by reflection name, enough to decode an attribute blob: an enum's size comes
    /// from its definition in this assembly, and an enum declared elsewhere cannot be decoded.</summary>
    sealed class AttributeTypes(MetadataReader md, Dictionary<string, TypeDefinitionHandle> byName) : ICustomAttributeTypeProvider<string>
    {
        public const string SystemType = "System.Type";
        public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();
        public string GetSystemType() => SystemType;
        public bool IsSystemType(string type) => type == SystemType;
        public string GetTypeFromSerializedName(string name) => name;
        public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => ReflectionName(md, handle);
        public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => ReflectionName(md, handle);
        public string GetTypeFromSpecification(MetadataReader reader, object? context, TypeSpecificationHandle handle, byte rawTypeKind) => "?";
        public string GetSZArrayType(string elementType) => elementType + "[]";
        public PrimitiveTypeCode GetUnderlyingEnumType(string type)
        {
            // An enum's one instance field, `value__`, has the underlying primitive type.
            if (byName.TryGetValue(type, out var handle))
                foreach (var field in md.GetTypeDefinition(handle).GetFields().Select(md.GetFieldDefinition))
                    if ((field.Attributes & FieldAttributes.Static) == 0)
                    {
                        var signature = md.GetBlobReader(field.Signature);
                        signature.ReadSignatureHeader();
                        return (PrimitiveTypeCode)signature.ReadSignatureTypeCode();
                    }
            throw new NotSupportedException($"enum {type} is not declared in this assembly");
        }
    }

    /// <summary>Records each type definition of this assembly a signature names; the shape it builds is unused.</summary>
    sealed class Collector(MetadataReader md, HashSet<TypeDefinitionHandle> found) : ISignatureTypeProvider<int, object?>
    {
        public int GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            found.Add(TopLevel(md, handle));
            return 0;
        }
        public int GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => 0;
        public int GetTypeFromSpecification(MetadataReader reader, object? context, TypeSpecificationHandle handle, byte rawTypeKind) =>
            reader.GetTypeSpecification(handle).DecodeSignature(this, context);
        public int GetPrimitiveType(PrimitiveTypeCode typeCode) => 0;
        public int GetSZArrayType(int elementType) => 0;
        public int GetArrayType(int elementType, ArrayShape shape) => 0;
        public int GetByReferenceType(int elementType) => 0;
        public int GetPointerType(int elementType) => 0;
        public int GetGenericInstantiation(int genericType, System.Collections.Immutable.ImmutableArray<int> typeArguments) => 0;
        public int GetGenericTypeParameter(object? context, int index) => 0;
        public int GetGenericMethodParameter(object? context, int index) => 0;
        public int GetFunctionPointerType(MethodSignature<int> signature) => 0;
        public int GetModifiedType(int modifier, int unmodifiedType, bool isRequired) => 0;
        public int GetPinnedType(int elementType) => 0;
    }
}
