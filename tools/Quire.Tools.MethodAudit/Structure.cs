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

/// <summary>Reads <see cref="Structure"/> from method bodies, naming every referenced member as Mathcad would.</summary>
sealed class StructureReader(MetadataReader md, System.Reflection.PortableExecutable.PEReader pe, SigProvider decoder,
    Func<string, string> typeName, Func<int, string?> methodName)
{
    private static readonly Dictionary<ushort, OpCode> Codes = typeof(OpCodes)
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
    private static string Member(string name) => name.Contains('<') ? "<generated>" : name;

    /// <summary>Mathcad's name for a type: mirrored types map whole, else their outermost mirrored declaring type.</summary>
    private string Name(string type)
    {
        string mapped = typeName(type);
        int nest = type.IndexOf('+');
        if (mapped == type && nest > 0) mapped = typeName(type[..nest]) + type[nest..];
        return string.Join("+", mapped.Split('+').Select(Member));
    }
}
