using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using Quire.Tools.MirrorGen;

// `members ASSEMBLY TYPE`: the identity of an original artifact and the token and signature of every method of a type
// and its nested types, which an evidence record cites (tools/reference/reference.py).
if (args is ["members", var assemblyPath, var typeName])
{
    var artifact = new Artifact(assemblyPath);
    Console.WriteLine($"# {artifact.Name} {artifact.Version}\n# SHA-256: {artifact.Hash}\n# MVID: {artifact.Mvid}");
    var members = artifact.Methods.Where(m => m.Owner == typeName || m.Owner.StartsWith(typeName + "+")).ToList();
    foreach (var m in members)
        Console.WriteLine($"0x{m.Token:x8}\t{m.Owner}\t{m.Key}\t{m.Access}{(m.BodyRva == 0 ? "\tno body" : "")}");
    return members.Count > 0 ? 0 : 1;
}
var options = new Dictionary<string, List<string>>();
for (int i = 0; i < args.Length; i += 2)
{
    if (i + 1 >= args.Length || args[i] is not ("--repo" or "--assemblies" or "--install" or "--output" or "--project"))
        throw new ArgumentException("MethodAudit --repo DIR --assemblies DIR --install DIR --output FILE [--project NAME ...]");
    if (!options.TryGetValue(args[i], out var list)) options[args[i]] = list = [];
    list.Add(args[i + 1]);
}
string One(string name) => options.TryGetValue(name, out var values) && values.Count == 1
    ? Path.GetFullPath(values[0]) : throw new ArgumentException($"Exactly one {name} is required");
string repo = One("--repo"), assemblies = One("--assemblies"), install = One("--install"), output = One("--output");
var projects = options.GetValueOrDefault("--project") ?? Directory.GetFiles(Path.Combine(repo, "docs/mirror/manifest"), "*.txt")
    .Select(Path.GetFileNameWithoutExtension).OfType<string>().ToList();
var rows = new List<object>();
var errors = new List<string>();
var reviewed = new Dictionary<string, string[]>();
using (var csv = new TextFieldParser(Path.Combine(repo, "docs/mirror/type-map.csv")))
{
    csv.SetDelimiters(",");
    csv.HasFieldsEnclosedInQuotes = true;
    csv.ReadFields();
    while (!csv.EndOfData)
    {
        var fields = csv.ReadFields()!;
        if (fields[5] is "missing" or "exists" or "exists-elsewhere")
            reviewed[fields[4]] = [fields[1], Path.GetFileNameWithoutExtension(fields[0])];
    }
}
var originals = new Dictionary<string, Artifact>();
var quire = new List<Artifact>();
foreach (string project in projects.Distinct())
{
    string path = Path.Combine(assemblies, project + ".dll");
    try { quire.Add(new Artifact(path)); }
    catch (Exception e) { errors.Add($"{project}: cannot inspect required assembly: {e.Message}"); }
}
foreach (var artifact in quire)
    foreach (var t in artifact.Model.Types.Where(t => t.MirrorOf != null))
    {
        var binding = t.MirrorOf!.Split(',').Select(s => s.Trim()).ToArray();
        if (!reviewed.TryGetValue(t.FullName, out var expected) || binding.Length < 2
            || binding[0] != expected[0] || binding[1] != expected[1])
        {
            errors.Add($"{t.FullName}: MirrorOf does not match the reviewed type-map binding");
            continue;
        }
        string assembly = binding[1];
        if (originals.ContainsKey(assembly)) continue;
        try { originals[assembly] = new Artifact(Path.Combine(install, assembly + ".dll")); }
        catch (Exception e) { errors.Add($"{assembly}: original unavailable: {e.Message}"); }
    }
var typeNames = quire.SelectMany(a => a.Model.Types).Where(t => t.MirrorOf != null)
    .GroupBy(t => t.FullName).ToDictionary(g => g.Key, g => g.First().MirrorOf!.Split(',').Select(s => s.Trim()).ToArray());
TypeSig Map(TypeSig sig) => sig switch
{
    NamedSig n => n with { Name = typeNames.TryGetValue(n.Name, out var names) ? names[0] : n.Name,
        Assembly = typeNames.TryGetValue(n.Name, out var owner) ? owner[1] : n.Assembly, Args = [.. n.Args.Select(Map)] },
    ArraySig a => a with { Element = Map(a.Element) },
    ByRefSig b => b with { Element = Map(b.Element) },
    PointerSig p => p with { Element = Map(p.Element) },
    _ => sig
};
string Key(MethodModel m, bool map) => Signatures.Method(m.Name, m.GenericParams.Length, m.Params.Select(p => map ? Map(p.Type) : p.Type), map ? Map(m.Return) : m.Return, m.Static);
var matched = new List<(Artifact Artifact, RawMethod Raw, TypeModel Type, MethodModel Method, IHasStatus Status, string? Exclusion, Artifact? Original, RawMethod? Counterpart)>();
foreach (var artifact in quire)
    foreach (var raw in artifact.Methods)
    {
        var type = artifact.Model.Types.Single(t => t.FullName == raw.Owner);
        // Only names a compiler can produce: [CompilerGenerated] on a hand-written type must not hide its methods.
        string? exclusion = raw.Name.StartsWith('<') || type.CompilerGenerated && type.FullName.Contains('<')
            ? "compiler-generated" : null;
        var candidates = type.Methods.Concat(type.Properties.SelectMany(p => new[] { p.Getter, p.Setter }))
            .Concat(type.Events.SelectMany(e => new[] { e.Adder, e.Remover })).OfType<MethodModel>();
        var method = candidates.Single(m => Key(m, false) == raw.Key);
        IHasStatus status = method;
        if (method.Status == MemberStatus.Unclassified)
        {
            var parent = type.Properties.FirstOrDefault(p => p.Getter == method || p.Setter == method) as IHasStatus
                ?? type.Events.FirstOrDefault(e => e.Adder == method || e.Remover == method);
            if (parent != null) status = parent;
        }
        RawMethod? counterpart = null;
        Artifact? original = null;
        if (type.MirrorOf is { } mirror)
        {
            string[] fields = mirror.Split(',');
            if (originals.TryGetValue(fields[1].Trim(), out original))
                counterpart = original.Methods.SingleOrDefault(m => m.Owner == fields[0].Trim() && m.Key == Key(method, true));
        }
        matched.Add((artifact, raw, type, method, status, exclusion, original, counterpart));
    }
// Calls to Quire methods are named after their Mathcad counterparts; types after their MirrorOf binding.
var originalNames = matched.Where(m => m.Counterpart != null)
    .ToDictionary(m => (m.Artifact.Name, m.Raw.Token), m => m.Counterpart!.Name);
string MathcadType(string name) => typeNames.TryGetValue(name, out var names) ? names[0] : name;
var structures = new Dictionary<Artifact, StructureReader>();
StructureReader Reader(Artifact a, bool isQuire) => structures.TryGetValue(a, out var reader) ? reader
    : structures[a] = new StructureReader(a.Metadata, a.Pe, new SigProvider(a.Name),
        isQuire ? MathcadType : name => name,
        isQuire ? token => originalNames.GetValueOrDefault((a.Name, token)) : _ => null);
foreach (var (artifact, raw, type, method, status, exclusion, original, counterpart) in matched)
{
    List<string>? differences = null;
    if (exclusion == null && counterpart is { BodyRva: not 0 } && raw.BodyRva != 0)
    {
        try
        {
            differences = Reader(artifact, true).Read(MetadataTokens.MethodDefinitionHandle(raw.Token))
                .Compare(Reader(original!, false).Read(MetadataTokens.MethodDefinitionHandle(counterpart.Token)));
        }
        catch (Exception e) when (e is BadImageFormatException or KeyNotFoundException or InvalidOperationException)
        {
            differences = [$"IL unreadable: {e.Message}"];
        }
    }
    rows.Add(new {
        id = artifact.Name + "::" + raw.Owner + "::" + raw.Key, project = artifact.Name, type = raw.Owner, signature = raw.Key, token = raw.Token, token_hex = $"0x{raw.Token:x8}",
        is_static = method.Static, is_constructor = method.IsConstructor, access = method.Access.ToString(), status = status.Status.ToString(), evidence = status.Evidence,
        quire_sha256 = artifact.Hash, quire_mvid = artifact.Mvid, exclusion,
        owner = exclusion == null ? null : Owner(raw.Owner, raw.Name),
        original = counterpart == null ? null : new { assembly = original!.Name, sha256 = original.Hash, mvid = original.Mvid,
            version = original.Version, token = counterpart.Token, token_hex = $"0x{counterpart.Token:x8}", type = counterpart.Owner, signature = counterpart.Key },
        declaration_only = counterpart is { BodyRva: 0 } && raw.BodyRva == 0
            && (method.Abstract && counterpart.Abstract || type.Kind == TypeKind.Delegate),
        shape_matches = counterpart != null && method.Access.ToString() == counterpart.Access,
        structure_matches = differences == null ? (bool?)null : differences.Count == 0,
        structure_differences = differences ?? [],
        source = raw.BodyRva == 0 ? null : artifact.Source(raw.Token),
        mutants = raw.BodyRva == 0 || exclusion != null && Owner(raw.Owner, raw.Name) == null ? []
            : Mutants.Find(artifact.Pe, artifact.Metadata.GetMethodDefinition(MetadataTokens.MethodDefinitionHandle(raw.Token)))
                .Select(m => new { il_offset = m.IlOffset, file_offset = m.FileOffset, original = m.Original,
                    replacement = m.Replacement, description = m.Description }).ToList()
    });
}
// Top-level types compiled into the differential test assembly, with the source files their methods come from, so
// the validator can reject any type an agent test file adds beside its own class (such as a shadowing Assert).
var testTypes = new List<object>();
string testAssembly = Path.Combine(assemblies, "Quire.Differential.Tests.dll");
if (File.Exists(testAssembly))
{
    using var pe = new PEReader(File.OpenRead(testAssembly));
    var md = pe.GetMetadataReader();
    string testPdb = Path.ChangeExtension(testAssembly, ".pdb");
    using var pdbProvider = File.Exists(testPdb) ? MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(testPdb)) : null;
    var pdb = pdbProvider?.GetMetadataReader();
    IEnumerable<string> Documents(TypeDefinition type) => type.GetMethods()
        .SelectMany(h => pdb == null ? [] : pdb.GetMethodDebugInformation(h.ToDebugInformationHandle()).GetSequencePoints())
        .Where(p => !p.IsHidden).Select(p => pdb!.GetString(pdb.GetDocument(p.Document).Name))
        .Concat(type.GetNestedTypes().SelectMany(n => Documents(md.GetTypeDefinition(n))));
    foreach (var handle in md.TypeDefinitions)
    {
        var type = md.GetTypeDefinition(handle);
        if (!type.GetDeclaringType().IsNil) continue;
        string name = md.GetString(type.Namespace) is { Length: > 0 } ns ? ns + "." + md.GetString(type.Name) : md.GetString(type.Name);
        if (name == "<Module>") continue;
        testTypes.Add(new { name, documents = Documents(type).Distinct().Order().ToArray() });
    }
}
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, JsonSerializer.Serialize(new { schema = 2, projects, errors, methods = rows, test_types = testTypes }, new JsonSerializerOptions { WriteIndented = true }) + "\n");
Console.WriteLine($"Method inventory: {rows.Count} declarations, {errors.Count} inspection errors; {output}");
return errors.Count == 0 && rows.Count > 0 ? 0 : 1;

// The hand-written method a compiler-generated one implements: a lambda or local function `<Name>b__…`/`<Name>g__…`
// (in the type or a `<>c…` closure class), or an iterator/async state machine `<Name>d__…`. Null for shared closure
// plumbing such as the constructors of `<>c`.
static object? Owner(string typeName, string methodName)
{
    var parts = typeName.Split('+');
    string type = string.Join('+', parts.TakeWhile(p => !p.StartsWith('<')));
    // The text inside the leading <...>, brackets matched by depth (`<Ns.IFoo<T>.Bar>b__0_0`), unwrapped again when it
    // is itself compiler-named (`<<Bar>g__Local|3_0>d` is the state machine of local function Local in Bar).
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
    return type.Length == 0 || name == null ? null : new { type, name };
}

sealed record RawMethod(string Owner, string Name, string Key, int Token, string Access, bool Abstract, int BodyRva);
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
                Methods.Add(new(owner, name, key, MetadataTokens.GetToken(mh), access,
                    (m.Attributes & MethodAttributes.Abstract) != 0, m.RelativeVirtualAddress));
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
