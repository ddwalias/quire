using Microsoft.VisualBasic.FileIO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
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
    if (args[i] == "--release-il")
    {
        options[args[i]] = ["true"];
        i--;
        continue;
    }
    if (i + 1 >= args.Length || args[i] is not ("--repo" or "--assemblies" or "--install" or "--output" or "--project" or "--type" or "--diff"))
        throw new ArgumentException("MethodAudit --repo DIR --assemblies RELEASE_DIR --install DIR (--output FILE | --diff KEY ...) [--project NAME ...] [--type TYPE ...] [--release-il]");
    if (!options.TryGetValue(args[i], out var list)) options[args[i]] = list = [];
    list.Add(args[i + 1]);
}
string One(string name) => options.TryGetValue(name, out var values) && values.Count == 1
    ? Path.GetFullPath(values[0]) : throw new ArgumentException($"Exactly one {name} is required");
// `--diff KEY`: instead of the inventory, print to stdout where each matching method's instructions first differ from
// Mathcad's (Structure.Diff); KEY is a type, `Type::Name` (every overload) or an inventory key `Type::signature`.
var diffs = options.GetValueOrDefault("--diff");
string repo = One("--repo"), assemblies = One("--assemblies"), install = One("--install");
string? output = diffs == null ? One("--output") : null;
int shown = 0;
var projects = options.GetValueOrDefault("--project") ?? Directory.GetFiles(Path.Combine(repo, "docs/mirror/manifest"), "*.txt")
    .Select(Path.GetFileNameWithoutExtension).OfType<string>().ToList();
var selectedTypes = options.GetValueOrDefault("--type")?.ToHashSet(StringComparer.Ordinal) ?? [];
var rows = new List<object>();
var errors = new List<string>();
// Quire type -> every (Mathcad type, Mathcad assembly) the type map binds it to: one Quire name stands for one type per
// Mathcad assembly when a source file is linked into each, or a native header compiled into several.
var reviewed = new Dictionary<string, List<string[]>>();
// (Mathcad assembly, Mathcad type) of every row the type map lists other than as a C++ artefact: those types are
// mirrored, or excluded for a reason a C++/CLI build cannot share (see the same-name binding below).
var listed = new HashSet<(string Assembly, string Type)>();
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
        if (fields[5] != "c++ artefact") listed.Add((Path.GetFileNameWithoutExtension(fields[0]).ToLowerInvariant(), fields[1]));
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
foreach (string project in projects.Distinct())
{
    try
    {
        var artifact = new Artifact(Path.Combine(assemblies, project + ".dll"));
        if (artifact.OptimizationsDisabled)
            throw new InvalidDataException("Debug assemblies are not accepted; rebuild with -c Release");
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
// A C++/CLI build emits the C++ types it uses (Windows SDK records, STL, CRT and template instances) under their C++
// names, where no [MirrorOf] can be written. In a mixed-mode Quire assembly such a type is the type of the same name, kind
// and native size in a Mathcad assembly the Quire assembly binds, when the type map lists that original as a C++ artefact
// or not at all (template instances and compiler-generated types, which it leaves out). A type the map mirrors or excludes
// otherwise still needs its reviewed [MirrorOf]; a type without such an original stays unbound and has no counterpart.
foreach (var artifact in quire.Where(a => !a.IlOnly))
{
    var bound = typeNames.Where(t => t.Key.Assembly == artifact.Name).Select(t => t.Value[1].ToLowerInvariant()).ToHashSet();
    foreach (var type in artifact.Model.Types.Where(t => t.MirrorOf == null))
    {
        if (reviewed.ContainsKey(type.FullName)) continue;
        var same = originals.Values.Where(o => bound.Contains(o.Name.ToLowerInvariant()) && !listed.Contains((o.Name.ToLowerInvariant(), type.FullName))
            && o.Model.Types.Any(t => t.FullName == type.FullName && t.Kind == type.Kind && t.NativeSize == type.NativeSize)).ToList();
        if (same.Count == 1) typeNames[(artifact.Name, type.FullName)] = [type.FullName, same[0].Name];
    }
}
// Synthesized types cannot carry a source MirrorOf. Inherit only an exact nested name
// under a reviewed parent, and require a compiler-generated counterpart in that artifact.
string[]? GeneratedBinding(Artifact artifact, TypeModel type)
{
    if (typeNames.TryGetValue((artifact.Name, type.FullName), out var own)) return own;
    // Roslyn emits this assembly-level container without a source type to inherit from.
    // When only one loaded Mathcad artifact has the matching synthesized container, bind
    // it by its compiler-generated metadata identity just like a nested generated helper.
    if (type.CompilerGenerated && type.DeclaringType == null && type.FullName == "<PrivateImplementationDetails>")
    {
        var candidates = originals.Values.Where(original => original.Model.Types.Any(t =>
            t.FullName == type.FullName && t.CompilerGenerated)).ToList();
        if (candidates.Count == 1)
            return typeNames[(artifact.Name, type.FullName)] = [type.FullName, candidates[0].Name];
    }
    if (!type.CompilerGenerated || type.DeclaringType is not { } parentName) return null;
    var parent = artifact.Model.Types.SingleOrDefault(t => t.FullName == parentName);
    if (parent == null || GeneratedBinding(artifact, parent) is not { } binding
        || !originals.TryGetValue(binding[1], out var original)) return null;
    string nested = binding[0] + type.FullName[parentName.Length..];
    if (!original.Model.Types.Any(t => t.FullName == nested && t.CompilerGenerated)) return null;
    return typeNames[(artifact.Name, type.FullName)] = [nested, binding[1]];
}
foreach (var artifact in quire)
    foreach (var type in artifact.Model.Types.Where(t => t.CompilerGenerated))
        GeneratedBinding(artifact, type);
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
string ComparisonKey(MethodModel m, bool map) => Signatures.Method(Canonical.MethodName(m, map ? Map : null),
    m.GenericParams.Length, m.Params.Select(p => map ? Map(p.Type) : p.Type), map ? Map(m.Return) : m.Return, m.Static);
var originalKeys = new Dictionary<(Artifact Artifact, int Token), string>();
string OriginalKey(Artifact artifact, RawMethod raw)
{
    if (originalKeys.TryGetValue((artifact, raw.Token), out var key)) return key;
    return originalKeys[(artifact, raw.Token)] = artifact.ComparisonKey(raw);
}
// The global functions of a mixed-mode Quire assembly pair with those of the one mixed-mode Mathcad assembly its types
// bind: <Module> carries no [MirrorOf], and that original is its only C++/CLI counterpart. Null otherwise.
Artifact? ModuleOriginal(Artifact artifact)
{
    if (artifact.IlOnly) return null;
    var mixed = typeNames.Where(t => t.Key.Assembly == artifact.Name).Select(t => originals.GetValueOrDefault(t.Value[1]))
        .OfType<Artifact>().Where(o => !o.IlOnly).Distinct().ToList();
    return mixed.Count == 1 ? mixed[0] : null;
}
// Keys leave out custom modifiers, which C# cannot write (C++/CLI's IsLong, IsConst...). They only break a tie between
// originals that differ in nothing else, for the one whose modifiers are the Quire method's.
RawMethod? Counterpart(RawMethod raw, Artifact original, List<RawMethod> sameKey)
{
    if (sameKey.Count > 1)
    {
        var tied = sameKey.Where(m => m.Modifiers == raw.Modifiers).ToList();
        if (tied.Count != 1)
            errors.Add($"{raw.Owner}::{raw.Key}: {sameKey.Count} methods of {original.Name} match and custom modifiers "
                + $"do not single one out: {string.Join(", ", sameKey.Select(m => $"0x{m.Token:x8}"))}");
        sameKey = tied.Count == 1 ? tied : [];
    }
    return sameKey.SingleOrDefault();
}
var matched = new List<(Artifact Artifact, RawMethod Raw, TypeModel? Type, MethodModel? Method, Artifact? Original, RawMethod? Counterpart)>();
foreach (var artifact in quire)
    foreach (var raw in artifact.Methods)
    {
        var type = artifact.Model.Types.SingleOrDefault(t => t.FullName == raw.Owner);
        // C++/CLI global functions and CRT support live on <Module>, which has no type model. A global function pairs
        // with the original's of the same mapped signature, and is compared like any other method.
        if (type == null)
        {
            if (raw.Owner != "<Module>") throw new InvalidDataException($"{artifact.Name}: no type model for {raw.Owner}");
            var module = ModuleOriginal(artifact);
            string key = Signatures.Method(raw.Name, raw.Signature.GenericParameterCount, raw.Signature.ParameterTypes.Select(Map),
                Map(raw.Signature.ReturnType), raw.Static);
            matched.Add((artifact, raw, null, null, module,
                module == null ? null : Counterpart(raw, module, [.. module.Methods.Where(m => m.Owner == raw.Owner && m.Key == key)])));
            continue;
        }
        var candidates = type.Methods.Concat(type.Properties.SelectMany(p => new[] { p.Getter, p.Setter }))
            .Concat(type.Events.SelectMany(e => new[] { e.Adder, e.Remover })).OfType<MethodModel>();
        var method = candidates.Single(m => Key(m, false) == raw.Key);
        RawMethod? counterpart = null;
        Artifact? original = null;
        if (typeNames.TryGetValue((artifact.Name, type.FullName), out var fields) && originals.TryGetValue(fields[1].Trim(), out original))
            counterpart = Counterpart(raw, original, [.. original.Methods.Where(m => m.Owner == fields[0].Trim()
                && OriginalKey(original, m) == ComparisonKey(method, true))]);
        matched.Add((artifact, raw, type, method, original, counterpart));
    }
// Keys omit custom modifiers, so C++/CLI global overloads can share one: those add their modifiers to their identity,
// and the token only where that still leaves a tie.
var globalIds = new Dictionary<(Artifact, int), string>();
foreach (var group in matched.Where(m => m.Type == null).GroupBy(m => (m.Artifact, m.Raw.Key)))
    foreach (var byModifiers in group.GroupBy(m => m.Raw.Modifiers))
        foreach (var m in byModifiers)
            globalIds[(m.Artifact, m.Raw.Token)] = m.Raw.Key + (group.Count() == 1 ? "" : $" [{m.Raw.Modifiers}]")
                + (byModifiers.Count() == 1 ? "" : $"@0x{m.Raw.Token:x8}");
// Calls to Quire methods are named after their Mathcad counterparts; types after their MirrorOf binding.
var originalNames = matched.Where(m => m.Counterpart != null)
    .ToDictionary(m => (m.Artifact.Name, m.Raw.Token), m => m.Counterpart!.Name);

var structures = new Dictionary<Artifact, StructureReader>();
StructureReader Reader(Artifact a, bool isQuire) => structures.TryGetValue(a, out var reader) ? reader
    : structures[a] = new StructureReader(a.Metadata, a.Pe, new SigProvider(a.Name),
        isQuire ? name => (Binding(a.Name, name)?[0] ?? name) : name => name,
        isQuire ? token => originalNames.GetValueOrDefault((a.Name, token)) : _ => null, exact: true);
// Stable body hashes identify changed Release methods independently of metadata token numbering.
var bodies = quire.ToDictionary(a => a, a => new StructureReader(a.Metadata, a.Pe, new SigProvider(a.Name), name => name, _ => null, exact: true, root: repo));
// D19: an unpaired mixed-mode member whose bound Mathcad assembly has a member of the same owner, name and arity is a
// pairing miss, not compiler support Mathcad's build lacked.
bool OriginalNameTaken(Artifact artifact, RawMethod raw, TypeModel? type, Artifact? original, RawMethod? counterpart)
{
    if (artifact.IlOnly || counterpart != null) return false;
    var mixed = original ?? ModuleOriginal(artifact);
    if (mixed == null) return false;
    string owner = type != null && typeNames.TryGetValue((artifact.Name, type.FullName), out var fields) ? fields[0].Trim() : raw.Owner;
    int arity = raw.Signature.ParameterTypes.Length;
    return mixed.Methods.Any(m => m.Owner == owner && m.Name == raw.Name && m.Signature.ParameterTypes.Length == arity);
}
foreach (var (artifact, raw, type, method, original, counterpart) in matched)
{
    if (selectedTypes.Count > 0 && !selectedTypes.Contains(raw.Owner)) continue;
    string identity = method == null ? globalIds[(artifact, raw.Token)] : artifact.ComparisonKey(raw);
    bool wanted = diffs != null && diffs.Any(d => d == raw.Owner || d == $"{raw.Owner}::{raw.Name}"
        || d == $"{raw.Owner}::{raw.Key}" || method != null && d == $"{raw.Owner}::{Canonical.MethodName(method)}" || d == $"{raw.Owner}::{identity}");
    if (diffs != null && !wanted) continue;
    var body = raw.BodyRva == 0 || raw.NativeCode ? null : bodies[artifact].Body(MetadataTokens.MethodDefinitionHandle(raw.Token));
    List<string>? differences = null, diff = null;
    bool exact = false;
    if (counterpart is { BodyRva: not 0, NativeCode: false } && raw.BodyRva != 0 && !raw.NativeCode)
    {
        try
        {
            var mine = Reader(artifact, true).Read(MetadataTokens.MethodDefinitionHandle(raw.Token), true);
            var theirs = Reader(original!, false).Read(MetadataTokens.MethodDefinitionHandle(counterpart.Token), true);
            differences = mine.Compare(theirs);
            exact = differences.Count == 0 && mine.Exact(theirs);
            if (wanted) diff = mine.Diff(theirs, 3, offset => artifact.Line(raw.Token, offset) is { } line ? Path.GetRelativePath(repo, line) : null);
        }
        catch (Exception e) when (e is BadImageFormatException or KeyNotFoundException or InvalidOperationException)
        {
            differences = [$"IL unreadable: {e.Message}"];
        }
    }
    // A P/Invoke has no IL body: its metadata is the whole member. Its name and parameter types already paired it.
    else if (counterpart != null && (raw.Import != null || counterpart.Import != null))
    {
        differences = raw.Import == null || counterpart.Import == null
            ? [$"P/Invoke in {(raw.Import == null ? "Mathcad" : "Quire")} only"]
            : [.. raw.Import.Differences(counterpart.Import)];
        if (raw.Modifiers != counterpart.Modifiers) differences.Add($"custom modifiers {raw.Modifiers} vs {counterpart.Modifiers}");
        exact = differences.Count == 0;
    }
    if (wanted)
    {
        shown++;
        Console.WriteLine($"{raw.Owner}::{raw.Key}" + (counterpart == null ? "" : $" (Mathcad {original!.Name} 0x{counterpart.Token:x8})") + ": "
            + (differences == null ? "not compared: " + (counterpart == null ? "no Mathcad counterpart" : "no IL body")
               : exact ? "exact" : differences.Count == 0 ? "structure matches; instructions differ" : string.Join("; ", differences)));
        foreach (var line in diff ?? []) Console.WriteLine("  " + line);
        continue;
    }
    rows.Add(new {
        // Stable across explicit-interface source qualifiers; retain the literal metadata
        // signature separately so a spelling change does not look like a removed method.
        id = artifact.Name + "::" + raw.Owner + "::" + identity, project = artifact.Name,
        type = raw.Owner, signature = identity, declared_signature = raw.Key,
        token = raw.Token, token_hex = $"0x{raw.Token:x8}",
        is_static = raw.Static, is_constructor = method?.IsConstructor == true, access = method?.Access.ToString() ?? raw.Access,
        quire_sha256 = artifact.Hash, quire_mvid = artifact.Mvid,
        original = counterpart == null ? null : new { assembly = original!.Name, sha256 = original.Hash, mvid = original.Mvid,
            version = original.Version, token = counterpart.Token, token_hex = $"0x{counterpart.Token:x8}", type = counterpart.Owner, signature = counterpart.Key,
            native_code = counterpart.NativeCode },
        // D19: a C++/CLI candidate; the gate classifies its compiler-support and native-vs-native rows.
        mixed_mode = !artifact.IlOnly,
        original_name_taken = OriginalNameTaken(artifact, raw, type, original, counterpart),
        // D10: utl-mgd is a mixed-mode engine boundary. Matched bodies with a 0-7 original
        // maxstack and emitted bodies lacking a managed signature match are native routines.
        // An IL-only port cannot reproduce those bodies; a mixed-mode (C++/CLI) candidate can, so it gets no exemption.
        engine_native = artifact.IlOnly && type != null && typeNames.TryGetValue((artifact.Name, type.FullName), out var nativeBinding)
            && nativeBinding[1].Equals("utl-mgd", StringComparison.OrdinalIgnoreCase)
            && raw.BodyRva != 0
            && (counterpart == null || original?.Name.Equals("utl-mgd", StringComparison.OrdinalIgnoreCase) == true
                && original.Methods.Single(m => m.Token == counterpart.Token).MaxStack is >= 0 and <= 7),
        declaration_only = counterpart is { BodyRva: 0 } && raw.BodyRva == 0
            && (raw.Abstract && counterpart.Abstract || type?.Kind == TypeKind.Delegate),
        shape_matches = counterpart != null && (method?.Access.ToString() ?? raw.Access) == counterpart.Access && raw.Modifiers == counterpart.Modifiers,
        structure_matches = differences == null ? (bool?)null : differences.Count == 0,
        structure_differences = differences ?? [],
        // Same instructions in the same order (Structure.Exact); null where the structure is not compared.
        exact_match = differences == null ? (bool?)null : exact,
        unsupported = raw.NativeCode ? "native code body has no IL to compare"
            : type == null && original == null ? "global <Module> method has no reviewed Mathcad binding" : null,
        source = raw.BodyRva == 0 ? null : artifact.Source(raw.Token),
        body_sha256 = body?.Hash,
        // A MirrorStub throw: unfinished, whichever file it records.
        stub_body = body?.Stub == true,
        // Generated: [MirrorStub] on the method, or on the property or event it is an accessor of.
        mirror_stub = method != null && type != null && (method.Stub
            || type.Properties.Any(p => p.Stub && (Same(p.Getter, method) || Same(p.Setter, method)))
            || type.Events.Any(e => e.Stub && (Same(e.Adder, method) || Same(e.Remover, method))))
    });
}
static bool Same(MethodModel? accessor, MethodModel method) =>
    accessor != null && accessor.Name == method.Name && accessor.Params.Length == method.Params.Length
    && accessor.Params.Zip(method.Params).All(p => p.First.Type.ToString() == p.Second.Type.ToString());

if (output == null)
{
    if (shown == 0) Console.Error.WriteLine($"No method matches {string.Join(", ", diffs!)}");
    foreach (var error in errors) Console.Error.WriteLine(error);
    return shown > 0 && errors.Count == 0 ? 0 : 1;
}
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, JsonSerializer.Serialize(new { schema = 2, projects, errors, methods = rows }, new JsonSerializerOptions { WriteIndented = true }) + "\n");
Console.WriteLine($"Method inventory: {rows.Count} declarations, {errors.Count} inspection errors; {output}");
return errors.Count == 0 && rows.Count > 0 ? 0 : 1;
