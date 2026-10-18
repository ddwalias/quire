using System.Reflection;
using System.Reflection.Emit;
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
    if (args[i] == "--inventory-only")
    {
        options[args[i]] = ["true"];
        continue;
    }
    if (i + 1 >= args.Length || args[i] is not ("--repo" or "--assemblies" or "--structure-assemblies" or "--install" or "--output" or "--project" or "--type"))
        throw new ArgumentException("MethodAudit --repo DIR --assemblies DIR [--structure-assemblies DIR] --install DIR --output FILE [--project NAME ...] [--type TYPE ...] [--inventory-only]");
    if (!options.TryGetValue(args[i], out var list)) options[args[i]] = list = [];
    list.Add(args[i + 1]);
}
string One(string name) => options.TryGetValue(name, out var values) && values.Count == 1
    ? Path.GetFullPath(values[0]) : throw new ArgumentException($"Exactly one {name} is required");
string repo = One("--repo"), assemblies = One("--assemblies"), install = One("--install"), output = One("--output");
// An optimized build of the same sources, like Mathcad's: only method structure is read from it.
string? structureAssemblies = options.ContainsKey("--structure-assemblies") ? One("--structure-assemblies") : null;
var projects = options.GetValueOrDefault("--project") ?? Directory.GetFiles(Path.Combine(repo, "docs/mirror/manifest"), "*.txt")
    .Select(Path.GetFileNameWithoutExtension).OfType<string>().ToList();
var selectedTypes = options.GetValueOrDefault("--type")?.ToHashSet(StringComparer.Ordinal) ?? [];
var inventoryOnly = options.ContainsKey("--inventory-only");
var rows = new List<object>();
var errors = new List<string>();
// Quire type -> every (Mathcad type, Mathcad assembly) the type map binds it to: one Quire name stands for one type per
// Mathcad assembly when a source file is linked into each, or a native header compiled into several.
var reviewed = new Dictionary<string, List<string[]>>();
using (var csv = new TextFieldParser(Path.Combine(repo, "docs/mirror/type-map.csv")))
{
    csv.SetDelimiters(",");
    csv.HasFieldsEnclosedInQuotes = true;
    csv.ReadFields();
    while (!csv.EndOfData)
    {
        var fields = csv.ReadFields()!;
        if (fields[5] is "missing" or "exists" or "exists-elsewhere" or "shared source (linked into each assembly)"
            or "duplicate (shared native header)")
        {
            if (!reviewed.TryGetValue(fields[4], out var bound)) reviewed[fields[4]] = bound = [];
            bound.Add([fields[1], Path.GetFileNameWithoutExtension(fields[0])]);
        }
    }
}
// Windows names assemblies case-insensitively; the type map records install file names, not assembly names.
bool Reviewed(string quireType, string[] binding) => binding.Length >= 2 && reviewed.TryGetValue(quireType, out var bound)
    && bound.Any(b => b[0] == binding[0] && string.Equals(b[1], binding[1], StringComparison.OrdinalIgnoreCase));
string InstallFile(string assembly)
{
    var found = Directory.EnumerateFiles(install, "*.dll")
        .Where(f => string.Equals(Path.GetFileNameWithoutExtension(f), assembly, StringComparison.OrdinalIgnoreCase)).ToList();
    return found.Count == 1 ? found[0]
        : throw new FileNotFoundException($"{(found.Count == 0 ? "no" : "several")} {assembly}.dll in {install}");
}
var originals = new Dictionary<string, Artifact>(StringComparer.OrdinalIgnoreCase);
var quire = new List<Artifact>();
var optimized = new Dictionary<Artifact, (Artifact Build, Dictionary<(string, string), int> Tokens)>();
foreach (string project in projects.Distinct())
{
    try
    {
        var artifact = new Artifact(Path.Combine(assemblies, project + ".dll"));
        if (structureAssemblies != null)
        {
            var built = new Artifact(Path.Combine(structureAssemblies, project + ".dll"));
            // Closures, state machines and helpers the compiler generates differ between builds; nothing else may.
            static RawMethod? OnlyIn(Artifact a, Artifact other)
            {
                var keys = other.Methods.Select(m => (m.Owner, m.Key)).ToHashSet();
                return a.Methods.FirstOrDefault(m => !m.Owner.Contains('<') && !m.Name.StartsWith('<') && !keys.Contains((m.Owner, m.Key)));
            }
            static string Describe(RawMethod? m) => m == null ? "none" : $"{m.Owner}::{m.Key} (0x{m.Token:x8})";
            RawMethod? debug = OnlyIn(artifact, built), release = OnlyIn(built, artifact);
            if (debug != null || release != null)
                throw new InvalidDataException($"the optimized build declares other methods: only in Debug {Describe(debug)}, only optimized {Describe(release)}");
            if (CompiledAway(artifact) is { } reason)
                throw new InvalidDataException($"the optimized build compiles away code the tests run: {reason}");
            optimized[artifact] = (built, built.Methods.ToDictionary(m => (m.Owner, m.Key), m => m.Token));
        }
        quire.Add(artifact);
    }
    catch (Exception e) { errors.Add($"{project}: cannot inspect required assembly: {e.Message}"); }
}
// Each Quire copy of a name bound in several Mathcad assemblies claims its own binding. Two copies claiming one would
// both pair with that original and leave another unchecked.
var claims = new Dictionary<(string Name, string Type, string Assembly), (string Assembly, List<string> Projects)>();
foreach (var artifact in quire)
    foreach (var t in artifact.Model.Types.Where(t => t.MirrorOf != null))
    {
        var binding = t.MirrorOf!.Split(',').Select(s => s.Trim()).ToArray();
        if (!Reviewed(t.FullName, binding))
        {
            errors.Add($"{t.FullName}: MirrorOf does not match the reviewed type-map binding");
            continue;
        }
        string assembly = binding[1];
        if (reviewed[t.FullName].Count > 1)
        {
            var key = (t.FullName, binding[0], assembly.ToLowerInvariant());
            if (!claims.TryGetValue(key, out var claim)) claims[key] = claim = (assembly, []);
            claim.Projects.Add(artifact.Name);
        }
        if (originals.ContainsKey(assembly)) continue;
        try { originals[assembly] = new Artifact(InstallFile(assembly)); }
        catch (Exception e) { errors.Add($"{assembly}: original unavailable: {e.Message}"); }
    }
foreach (var ((name, type, _), (assembly, claimants)) in claims.Where(c => c.Value.Projects.Count > 1))
    errors.Add($"{name}: {string.Join(", ", claimants)} all claim the binding {type}, {assembly}; each copy binds its own");
// A reviewed binding no copy claims is not an error: Quire keeps one copy of a type a native header duplicates into
// several Mathcad assemblies (one [MirrorOf]), so the other assemblies' bindings stay unclaimed.
// Bindings by declaring Quire assembly and type: the same name may be bound differently in each project.
var typeNames = new Dictionary<(string Assembly, string Name), string[]>();
foreach (var artifact in quire)
    foreach (var t in artifact.Model.Types.Where(t => t.MirrorOf != null))
        typeNames.TryAdd((artifact.Name, t.FullName), t.MirrorOf!.Split(',').Select(s => s.Trim()).ToArray());
var referenced = new Dictionary<string, AssemblyModel?>();
var assemblyNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
// A type of a project not audited here (one a signature or a call names) maps through the reviewed type map, as its
// own [MirrorOf] would; the audited projects' bindings were checked against it above. Where the type map binds the
// name in several assemblies, the [MirrorOf] of the Quire assembly declaring it decides, if it is a reviewed binding.
// The assembly is named as its metadata names it, which the type map's file name may not match in case.
string[]? Binding(string assembly, string name)
{
    if (typeNames.TryGetValue((assembly, name), out var own)) return own;
    if (!reviewed.TryGetValue(name, out var bound)) return null;
    string[]? binding = bound.Count == 1 ? bound[0] : null;
    if (binding == null)
    {
        if (!referenced.TryGetValue(assembly, out var model))
        {
            string path = Path.Combine(assemblies, assembly + ".dll");
            referenced[assembly] = model = File.Exists(path) ? MetadataLoader.Load(path) : null;
        }
        binding = model?.Types.FirstOrDefault(t => t.FullName == name)?.MirrorOf?.Split(',').Select(s => s.Trim()).ToArray();
        if (binding == null || !Reviewed(name, binding)) return null;
    }
    if (!assemblyNames.TryGetValue(binding[1], out var canonical))
    {
        try { canonical = originals.TryGetValue(binding[1], out var loaded) ? loaded.Name : MetadataLoader.Load(InstallFile(binding[1]))?.Name ?? binding[1]; }
        catch (Exception e) when (e is IOException or BadImageFormatException) { canonical = binding[1]; }
        assemblyNames[binding[1]] = canonical;
    }
    return [binding[0], canonical];
}
TypeSig Map(TypeSig sig) => sig switch
{
    NamedSig n => Binding(n.Assembly, n.Name) is { } b ? n with { Name = b[0], Assembly = b[1], Args = [.. n.Args.Select(Map)] }
        : n with { Args = [.. n.Args.Select(Map)] },
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
            {
                var sameKey = original.Methods.Where(m => m.Owner == fields[0].Trim() && m.Key == Key(method, true)).ToList();
                // Keys leave out custom modifiers, which C# cannot write (C++/CLI's IsLong, IsConst...). They only break a
                // tie between originals that differ in nothing else, for the one whose modifiers are the Quire method's.
                if (sameKey.Count > 1)
                {
                    var tied = sameKey.Where(m => m.Modifiers == raw.Modifiers).ToList();
                    if (tied.Count != 1)
                        errors.Add($"{raw.Owner}::{raw.Key}: {sameKey.Count} methods of {original.Name} match and custom modifiers "
                            + $"do not single one out: {string.Join(", ", sameKey.Select(m => $"0x{m.Token:x8}"))}");
                    sameKey = tied.Count == 1 ? tied : [];
                }
                counterpart = sameKey.SingleOrDefault();
            }
        }
        matched.Add((artifact, raw, type, method, status, exclusion, original, counterpart));
    }
// Calls to Quire methods are named after their Mathcad counterparts; types after their MirrorOf binding.
// The token a method has in the build its structure is read from; null when the optimized build lacks it.
int? StructureToken(Artifact a, RawMethod raw) => !optimized.TryGetValue(a, out var o) ? raw.Token
    : o.Tokens.TryGetValue((raw.Owner, raw.Key), out int token) ? token : null;
var originalNames = matched.Where(m => m.Counterpart != null && StructureToken(m.Artifact, m.Raw) != null)
    .ToDictionary(m => (m.Artifact.Name, StructureToken(m.Artifact, m.Raw)!.Value), m => m.Counterpart!.Name);

var structures = new Dictionary<Artifact, StructureReader>();
StructureReader Reader(Artifact a, bool isQuire) => structures.TryGetValue(a, out var reader) ? reader
    : structures[a] = new StructureReader(a.Metadata, a.Pe, new SigProvider(a.Name),
        isQuire ? name => (Binding(a.Name, name)?[0] ?? name) : name => name,
        isQuire ? token => originalNames.GetValueOrDefault((a.Name, token)) : _ => null);
// Quire's own names, unmapped and exact: what the body hash and call graph identify; the checkout path left out.
var bodies = quire.ToDictionary(a => a, a => new StructureReader(a.Metadata, a.Pe, new SigProvider(a.Name), name => name, _ => null, exact: true, root: repo));
foreach (var (artifact, raw, type, method, status, exclusion, original, counterpart) in matched)
{
    if (selectedTypes.Count > 0 && !selectedTypes.Contains(raw.Owner)) continue;
    var body = raw.BodyRva == 0 ? null : bodies[artifact].Body(MetadataTokens.MethodDefinitionHandle(raw.Token));
    bool isVirtual = (artifact.Metadata.GetMethodDefinition(MetadataTokens.MethodDefinitionHandle(raw.Token)).Attributes
        & MethodAttributes.Virtual) != 0;
    List<string>? differences = null;
    bool exact = false;
    if (!inventoryOnly && exclusion == null && counterpart is { BodyRva: not 0 } && raw.BodyRva != 0)
    {
        try
        {
            if (StructureToken(artifact, raw) is not { } token) differences = ["not comparable: the optimized build lacks this method"];
            else
            {
                var mine = Reader(optimized.TryGetValue(artifact, out var o) ? o.Build : artifact, true).Read(MetadataTokens.MethodDefinitionHandle(token));
                var theirs = Reader(original!, false).Read(MetadataTokens.MethodDefinitionHandle(counterpart.Token));
                differences = mine.Compare(theirs);
                exact = differences.Count == 0 && mine.Exact(theirs);
            }
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
        // Same instructions in the same order (Structure.Exact); null where the structure is not compared.
        exact_match = differences == null ? (bool?)null : exact,
        source = raw.BodyRva == 0 ? null : artifact.Source(raw.Token),
        body_sha256 = body?.Hash, calls = body?.Calls ?? [], virtual_calls = body?.VirtualCalls ?? [],
        field_reads = body?.Reads ?? [], field_writes = body?.Writes ?? [], reference_reads = body?.ReferenceReads ?? [], is_virtual = isVirtual,
        slot = isVirtual ? bodies[artifact].Slot(MetadataTokens.MethodDefinitionHandle(raw.Token)) : null,
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
        testTypes.Add(new { name, documents = Documents(type).Distinct().Order().ToArray(),
            references = TestTypes.References(md, pe, handle).Select(h => TestTypes.Name(md, h)).Order().ToArray() });
    }
}
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, JsonSerializer.Serialize(new { schema = 2, projects, errors, methods = rows, test_types = testTypes }, new JsonSerializerOptions { WriteIndented = true }) + "\n");
Console.WriteLine($"Method inventory: {rows.Count} declarations, {errors.Count} inspection errors; {output}");
return errors.Count == 0 && rows.Count > 0 ? 0 : 1;

// [Conditional] code, and calls to Debug and Contract (whose methods are [Conditional]), exist in Debug builds only.
static string? CompiledAway(Artifact a)
{
    var md = a.Metadata;
    string Type(EntityHandle type) => type.Kind switch
    {
        HandleKind.TypeReference => md.GetTypeReference((TypeReferenceHandle)type) is var r
            ? $"{md.GetString(r.Namespace)}.{md.GetString(r.Name)}" : "",
        HandleKind.TypeDefinition => SigProvider.FullName(md, (TypeDefinitionHandle)type),
        _ => "",
    };
    string Owner(EntityHandle member) => member.Kind switch
    {
        HandleKind.MemberReference => Type(md.GetMemberReference((MemberReferenceHandle)member).Parent),
        HandleKind.MethodDefinition => Type(md.GetMethodDefinition((MethodDefinitionHandle)member).GetDeclaringType()),
        HandleKind.MethodSpecification => Owner(md.GetMethodSpecification((MethodSpecificationHandle)member).Method),
        _ => "",
    };
    foreach (var handle in md.CustomAttributes)
    {
        var attribute = md.GetCustomAttribute(handle);
        if (Owner(attribute.Constructor) != "System.Diagnostics.ConditionalAttribute") continue;
        return attribute.Parent.Kind switch
        {
            HandleKind.MethodDefinition => a.Methods.Where(m => m.Token == MetadataTokens.GetToken(attribute.Parent))
                .Select(m => $"{m.Owner}::{m.Key} is [Conditional]").Single(),
            HandleKind.TypeDefinition => $"{Type(attribute.Parent)} is [Conditional]",
            _ => "a [Conditional] attribute",
        };
    }
    foreach (var m in a.Methods.Where(m => m.BodyRva != 0))
    {
        var il = a.Pe.GetMethodBody(m.BodyRva).GetILReader();
        while (il.RemainingBytes > 0)
        {
            byte first = il.ReadByte();
            var op = StructureReader.Codes[first == 0xFE ? (ushort)(0xFE00 | il.ReadByte()) : first];
            if (op.OperandType is not (OperandType.InlineMethod or OperandType.InlineTok)) { Mutants.Skip(ref il, op); continue; }
            if (Owner(MetadataTokens.EntityHandle(il.ReadInt32())) is ("System.Diagnostics.Debug" or "System.Diagnostics.Contracts.Contract") and var type)
                return $"{m.Owner}::{m.Key} calls {type}";
        }
    }
    return null;
}

// The hand-written method a compiler-generated one implements (CompilerGenerated.Owner), as the inventory spells it.
static object? Owner(string typeName, string methodName) =>
    CompilerGenerated.Owner(typeName, methodName) is { } owner ? new { type = owner.Type, name = owner.Name } : null;
