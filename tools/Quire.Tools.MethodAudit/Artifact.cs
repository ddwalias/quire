using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using Quire.Tools.MirrorGen;

/// <summary><paramref name="Modifiers"/>: the custom modifiers of the return type and each parameter, which the key leaves
/// out (see ModifierProvider).</summary>
sealed record RawMethod(string Owner, string Name, string Key, int Token, string Access, bool Abstract, int BodyRva,
    string Modifiers = "");
sealed class Artifact
{
    public AssemblyModel Model { get; }
    public string Name => Model.Name;
    public string Hash { get; }
    public string Mvid { get; }
    public string Version { get; }
    public List<RawMethod> Methods { get; } = [];
    public PEReader Pe { get; }
    private readonly MetadataReader? _debug;

    /// <summary>Statement lines of a method per source file, from its portable PDB, with the checksum the compiler
    /// recorded for the file it read; null without one.</summary>
    public object? Source(int token)
    {
        if (_debug == null) return null;
        var points = _debug.GetMethodDebugInformation(MetadataTokens.MethodDefinitionHandle(token).ToDebugInformationHandle())
            .GetSequencePoints().Where(p => !p.IsHidden).ToList();
        if (points.Count == 0) return null;
        // Constructors can span documents (field initializers in another partial file).
        return points.GroupBy(p => p.Document).Select(g =>
        {
            var document = _debug.GetDocument(g.Key);
            return new
            {
                file = _debug.GetString(document.Name),
                lines = g.Select(p => p.StartLine).Distinct().Order().ToArray(),
                checksum = new { algorithm = HashAlgorithmName(_debug.GetGuid(document.HashAlgorithm)),
                                 hash = Convert.ToHexString(_debug.GetBlobBytes(document.Hash)).ToLowerInvariant() },
            };
        }).ToArray();
    }

    /// <summary>`file:line` of the statement holding the instruction at IL <paramref name="offset"/> of a method, from
    /// the last visible sequence point at or before it; null without a PDB or such a point.</summary>
    public string? Line(int token, int offset)
    {
        var point = _debug?.GetMethodDebugInformation(MetadataTokens.MethodDefinitionHandle(token).ToDebugInformationHandle())
            .GetSequencePoints().Where(p => !p.IsHidden && p.Offset <= offset).Select(p => (SequencePoint?)p).LastOrDefault();
        return point is { } p ? $"{_debug!.GetString(_debug.GetDocument(p.Document).Name)}:{p.StartLine}" : null;
    }

    /// <summary>The hashlib name of a portable PDB document checksum algorithm; the GUID itself when unknown.</summary>
    static string HashAlgorithmName(Guid id) => id.ToString() switch
    {
        "8829d00f-11b8-4213-878b-770e8597ac16" => "sha256",
        "ff1816ec-aa5e-4d10-87f7-6f4963833460" => "sha1",
        var other => other,
    };
    public MetadataReader Metadata { get; }
    public Artifact(string path)
    {
        Model = MetadataLoader.Load(path) ?? throw new BadImageFormatException(path);
        byte[] image = File.ReadAllBytes(path);
        Hash = Convert.ToHexString(SHA256.HashData(image)).ToLowerInvariant();
        Pe = new PEReader(new MemoryStream(image));
        var md = Metadata = Pe.GetMetadataReader();
        string pdb = Path.ChangeExtension(path, ".pdb");
        if (File.Exists(pdb))
            _debug = MetadataReaderProvider.FromPortablePdbStream(new MemoryStream(File.ReadAllBytes(pdb))).GetMetadataReader();
        Mvid = md.GetGuid(md.GetModuleDefinition().Mvid).ToString();
        Version = md.GetAssemblyDefinition().Version.ToString();
        var decoder = new SigProvider(Name);
        foreach (var th in md.TypeDefinitions)
        {
            string owner = SigProvider.FullName(md, th);
            foreach (var mh in md.GetTypeDefinition(th).GetMethods())
            {
                var m = md.GetMethodDefinition(mh);
                var sig = m.DecodeSignature(decoder, null);
                string name = md.GetString(m.Name);
                bool isStatic = (m.Attributes & MethodAttributes.Static) != 0;
                string access = (m.Attributes & MethodAttributes.MemberAccessMask) switch {
                    MethodAttributes.Public => "Public", MethodAttributes.Private => "Private",
                    MethodAttributes.Assembly => "Internal", MethodAttributes.Family => "Protected",
                    MethodAttributes.FamANDAssem => "PrivateProtected", MethodAttributes.FamORAssem => "ProtectedInternal", _ => "Private" };
                string key = Signatures.Method(name, sig.GenericParameterCount, sig.ParameterTypes, sig.ReturnType, isStatic);
                var modified = m.DecodeSignature(new ModifierProvider(), null);
                string modifiers = string.Join(";", new[] { modified.ReturnType }.Concat(modified.ParameterTypes)
                    .Select(t => string.Join(" ", ModifierProvider.Modifier.Matches(t).Select(x => x.Value))));
                Methods.Add(new(owner, name, key, MetadataTokens.GetToken(mh), access,
                    (m.Attributes & MethodAttributes.Abstract) != 0, m.RelativeVirtualAddress, modifiers));
            }
        }
    }
}

static class Signatures
{
    public static string Method(string name, int arity, IEnumerable<TypeSig> parameters, TypeSig result, bool isStatic) =>
        $"{name}`{arity}({string.Join(",", parameters.Select(Type))})->{Type(result)}|{isStatic}";
    private static string Type(TypeSig signature) => signature switch
    {
        NamedSig n => $"[{n.Assembly}]{n.Name}" + (n.Args.IsEmpty ? "" : "<" + string.Join(",", n.Args.Select(Type)) + ">"),
        ArraySig a => Type(a.Element) + "[" + new string(',', Math.Max(0, a.Rank - 1)) + "]",
        ByRefSig b => Type(b.Element) + (b.ReadOnly ? "&in" : "&"),
        PointerSig p => Type(p.Element) + "*",
        _ => signature.ToString()
    };
}

/// <summary>A signature type written out with its custom modifiers, from which RawMethod.Modifiers takes them.</summary>
sealed class ModifierProvider : ISignatureTypeProvider<string, object?>
{
    public static readonly System.Text.RegularExpressions.Regex Modifier = new(@"mod(?:opt|req)\([^()]*\)");
    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) =>
        $"{unmodifiedType} {(isRequired ? "modreq" : "modopt")}({modifier})";
    public string GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode.ToString();
    public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) =>
        SigProvider.FullName(reader, handle);
    public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var reference = reader.GetTypeReference(handle);
        string ns = reader.GetString(reference.Namespace);
        return (ns.Length > 0 ? ns + "." : "") + reader.GetString(reference.Name);
    }
    public string GetTypeFromSpecification(MetadataReader reader, object? context, TypeSpecificationHandle handle, byte rawTypeKind) =>
        reader.GetTypeSpecification(handle).DecodeSignature(this, context);
    public string GetSZArrayType(string elementType) => elementType + "[]";
    public string GetArrayType(string elementType, ArrayShape shape) => elementType + "[" + new string(',', shape.Rank - 1) + "]";
    public string GetByReferenceType(string elementType) => elementType + "&";
    public string GetPointerType(string elementType) => elementType + "*";
    public string GetPinnedType(string elementType) => elementType;
    public string GetGenericInstantiation(string genericType, System.Collections.Immutable.ImmutableArray<string> typeArguments) =>
        genericType + "<" + string.Join(",", typeArguments) + ">";
    public string GetGenericMethodParameter(object? context, int index) => "!!" + index;
    public string GetGenericTypeParameter(object? context, int index) => "!" + index;
    public string GetFunctionPointerType(MethodSignature<string> signature) => "method";
}
