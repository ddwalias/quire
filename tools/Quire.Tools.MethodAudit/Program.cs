using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.Json;
using Quire.Tools.MirrorGen;

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
        string? exclusion = raw.Name.StartsWith('<') || type.CompilerGenerated ? "compiler-generated" : null;
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
        original = counterpart == null ? null : new { assembly = original!.Name, sha256 = original.Hash, mvid = original.Mvid,
            version = original.Version, token = counterpart.Token, token_hex = $"0x{counterpart.Token:x8}", type = counterpart.Owner, signature = counterpart.Key },
        declaration_only = counterpart is { BodyRva: 0 } && raw.BodyRva == 0
            && (method.Abstract && counterpart.Abstract || type.Kind == TypeKind.Delegate),
        shape_matches = counterpart != null && method.Access.ToString() == counterpart.Access,
        structure_matches = differences == null ? (bool?)null : differences.Count == 0,
        structure_differences = differences ?? []
    });
}
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, JsonSerializer.Serialize(new { schema = 2, projects, errors, methods = rows }, new JsonSerializerOptions { WriteIndented = true }) + "\n");
Console.WriteLine($"Method inventory: {rows.Count} declarations, {errors.Count} inspection errors; {output}");
return errors.Count == 0 && rows.Count > 0 ? 0 : 1;

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
    public MetadataReader Metadata { get; }
    public Artifact(string path)
    {
        Model = MetadataLoader.Load(path) ?? throw new BadImageFormatException(path);
        byte[] image = File.ReadAllBytes(path);
        Hash = Convert.ToHexString(SHA256.HashData(image)).ToLowerInvariant();
        Pe = new PEReader(new MemoryStream(image));
        var md = Metadata = Pe.GetMetadataReader();
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
