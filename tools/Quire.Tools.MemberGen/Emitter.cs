using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Quire.Tools.MirrorGen;

/// <summary>
/// Writes the mechanical stubs of one project: each member moves from <c>Generated/Mirror</c> (its whole
/// <c>[MirrorStub]</c> line pair deleted) into a hand-written partial of its type with the reconstructed body,
    /// together with the stub fields those bodies use. Returns a JSON report with original tokens
    /// that maps every written line back to its member. accept.py retains only Release IL matches.
/// Only members whose every method key (<c>Owner::Key</c>) is in <paramref name="allowed"/> are written.
/// </summary>
sealed partial class Emitter(string repo, World world, Artifact quire, string project, HashSet<string> skip, HashSet<string> allowed)
{
    private const string Stub = "[global::Quire.Mirror.MirrorStub]";
    public List<object> Emitted { get; } = [];
    public int Moved { get; private set; }

    /// <summary>Why a mechanical plan cannot be written; null when it can (used by the survey).</summary>
    public static string? RenderFailure(World world, Artifact quire, Plan plan)
    {
        try
        {
            foreach (var p in plan.Stub.Methods)
                if (plan.Bodies.TryGetValue(p.Role, out var b)) Write(world, new Names(plan.Stub.Type.Namespace, world), plan.Stub, p, b);
            return null;
        }
        catch (Exception e) when (e is NotMechanical or NotSupportedException) { return e.Message; }
    }

    private static Renderer.Text Write(World world, Names names, Stub stub, Pair p, (Body Body, bool Constructor) b)
    {
        names.TypeParameters = [.. stub.Type.GenericParams.Select(g => g.Name)];
        var enclosing = new List<string>();
        for (string? t = stub.Type.FullName; t != null; t = world.QuireTypes.GetValueOrDefault(t)?.DeclaringType) enclosing.Add(t);
        names.Enclosing = [.. enclosing];
        names.MethodParameters = [.. p.Method.GenericParams.Select(g => g.Name)];
        var parameters = p.Method.Params.Select(x => x.Name).ToArray();
        return new Renderer(world, names, stub.Type, parameters).Render(b.Body, b.Constructor);
    }

    /// <summary>A stub member's place in a generated file: the member line follows the attribute line.</summary>
    private sealed record Site(string File, int Line, string Text, string[] TypePath, string[] Kinds);

    public string Run()
    {
        var stubs = Stubs.Find(quire, world);
        var plans = stubs.Select(Stubs.Classify).ToList();
        var sites = Sites(stubs);
        var files = new Dictionary<string, List<string>>();   // generated file → its lines (CRLF kept)
        var deletions = new List<Site>();
        var members = new Dictionary<string, List<(string[] Path, string[] Kinds, List<string> Lines, object Report)>>();  // top-level type → members
        var namesByType = new Dictionary<string, Names>();
        foreach (var plan in plans.Where(p => p.Mechanical && !skip.Contains(p.Stub.Id)
                     && p.Stub.Methods.All(m => allowed.Contains($"{m.Quire.Owner}::{m.Quire.Key}"))))
        {
            var stub = plan.Stub;
            if (!sites.TryGetValue(stub, out var site)) continue;
            string top = site.TypePath[0];
            if (!namesByType.TryGetValue(top, out var names))
            {
                namesByType[top] = names = new Names(stub.Type.Namespace, world);
                if (Target(top) is { } existing) names.Import(File.ReadAllLines(existing).Where(l => l.StartsWith("using ") && l.EndsWith(';') && !l.Contains('='))
                    .Select(l => l["using ".Length..^1].Trim()));
            }
            const string evidence = "Release IL";
            List<string> lines;
            try { lines = Member(names, plan, site, evidence); }
            catch (Exception e) when (e is NotMechanical or NotSupportedException) { continue; }
            // Stub fields the bodies use move with them; the report names the member that moved each one, so a
            // compile error in a moved field rejects that member.
            var used = plan.Bodies.Values.SelectMany(b => Fields(b.Body)).Where(f => world.ToQuire(f.Owner) is NamedSig o && o.Name == stub.Type.FullName)
                .Select(f => f.Name).ToHashSet();
            foreach (var field in stubs.Where(s => s.Kind == "field" && s.Type == stub.Type && used.Contains(s.Name) && sites.ContainsKey(s) && !deletions.Contains(sites[s])))
            {
                var fs = sites[field];
                Add(members, top, fs, [$"[MirrorVerified(\"{evidence}\")]", Rewrite(names, fs.Text.Trim())],
                    new { id = field.Id, owner = stub.Id, kind = "field", type = field.Type.FullName, member = field.Name });
                deletions.Add(fs);
                Moved++;
            }
            var report = new
            {
                id = stub.Id, kind = stub.Kind, category = plan.Category, type = stub.Type.FullName, member = stub.Name,
                methods = stub.Methods.Select(p => $"{project}::{stub.Type.FullName}::{p.Quire.Key}").ToArray(),
                mathcad_tokens = stub.Methods.Select(p => $"0x{p.Mathcad!.Token:x8}").ToArray()
            };
            Add(members, top, site, [$"[MirrorVerified(\"{evidence}\")]", .. lines], report);
            deletions.Add(site);
            Emitted.Add(report);
        }
        // Delete the stub pairs, bottom-up within each file.
        foreach (var group in deletions.GroupBy(d => d.File))
        {
            var text = File.ReadAllText(group.Key);
            var lines = text.Split('\n').ToList();
            foreach (var d in group.OrderByDescending(d => d.Line)) lines.RemoveRange(d.Line - 1, 2);
            File.WriteAllText(group.Key, string.Join('\n', lines));
        }
        foreach (var (top, list) in members) WriteSource(top, list, namesByType[top]);
        var written = Written();
        // D17: IL acceptance emits no test classes or per-type evidence documents.
        return JsonSerializer.Serialize(new { project, members = Emitted, files = written }, new JsonSerializerOptions { WriteIndented = true });
    }

    private static void Add(Dictionary<string, List<(string[], string[], List<string>, object)>> members, string top, Site site, List<string> lines, object report)
    {
        if (!members.TryGetValue(top, out var list)) members[top] = list = [];
        list.Add((site.TypePath, site.Kinds, lines, report));
    }

    private static IEnumerable<FieldRef> Fields(Body body)
    {
        IEnumerable<Expr> All(Expr? e) => e switch
        {
            null => [],
            Field f => All(f.Target).Prepend(f),
            Call c => c.Args.SelectMany(All).Concat(All(c.Target)),
            New n => n.Args.SelectMany(All),
            NewArray a => All(a.Size),
            Element x => All(x.Array).Concat(All(x.Index)),
            Length l => All(l.Array),
            Conversion v => All(v.Value),
            As a => All(a.Value),
            Is i => All(i.Value),
            Unary u => All(u.Value),
            Binary b => All(b.Left).Concat(All(b.Right)),
            _ => [e],
        };
        return body.Statements.SelectMany(s => s switch
        {
            Assign a => All(a.Target).Concat(All(a.Value)),
            Do d => All(d.Value),
            Return r => All(r.Value),
            Throw t => All(t.Value),
            Initializer i => i.Args.SelectMany(All),
            _ => [],
        }).OfType<Field>().Select(f => f.F);
    }

    /// <summary>The member's C# lines: its stub declaration with the generated body replaced, types shortened.</summary>
    private List<string> Member(Names names, Plan plan, Site site, string testClass)
    {
        var stub = plan.Stub;
        string line = Rewrite(names, site.Text.Trim());
        switch (stub.Kind)
        {
            case "event":
                return line.Contains('{') ? throw new NotMechanical("event with accessors") : [line];
            case "property":
            {
                int brace = line.IndexOf(" { ", StringComparison.Ordinal);
                if (brace < 0) throw new NotMechanical("property form");
                string header = line[..brace];
                var accessors = AccessorPattern().Matches(line[brace..]).ToDictionary(m => m.Groups["kind"].Value, m => m.Groups["mod"].Value);
                if (plan.Category == "auto-property")
                    return [$"{header} {{ {string.Join(" ", accessors.Select(a => $"{a.Value}{a.Key};"))} }}"];
                var texts = stub.Methods.ToDictionary(p => p.Role, p => Write(world, names, stub, p, plan.Bodies[p.Role]));
                if (texts.Values.Any(t => t.Expression == null)) throw new NotMechanical("multi-statement accessor");
                if (texts.Count == 1 && texts.ContainsKey("get") && accessors.GetValueOrDefault("get") == "")
                    return [$"{header} => {texts["get"].Expression};"];
                return [$"{header} {{ {string.Join(" ", texts.Select(t => $"{accessors.GetValueOrDefault(t.Key)}{t.Key} => {t.Value.Expression};"))} }}"];
            }
            default:
            {
                var p = stub.Methods.Single();
                var text = Write(world, names, stub, p, plan.Bodies[p.Role]);
                string header = Header(line, stub.Kind == "constructor");
                if (text.Initializer != null) header += " " + text.Initializer;
                if (text.Expression != null) return [$"{header} => {text.Expression};"];
                if (text.Statements.Count == 0) return [header + " { }"];
                return [header, "{", .. text.Statements.Select(s => "    " + s), "}"];
            }
        }
    }

    /// <summary>A stub's declaration without its body (and a constructor's generated initializer).</summary>
    private static string Header(string line, bool constructor)
    {
        string header = line.EndsWith("{ }") ? line[..^3].TrimEnd()
            : line.Contains(" => ") ? line[..line.IndexOf(" => ", StringComparison.Ordinal)] : throw new NotMechanical("method form");
        if (constructor && Regex.Match(header, @"\s:\s(base|this)\(") is { Success: true } m) header = header[..m.Index];
        return header;
    }

    [GeneratedRegex(@"(?<mod>(?:private protected|protected internal|private|protected|internal)\s+)?(?<kind>get|set|init)\b")]
    private static partial Regex AccessorPattern();

    [GeneratedRegex(@"global::((?:@?[A-Za-z_]\w*\.)*@?[A-Za-z_]\w*)")]
    private static partial Regex Qualified();

    /// <summary>The generated declaration's <c>global::</c> names, as this file names them.</summary>
    private string Rewrite(Names names, string line) => Qualified().Replace(line, m =>
    {
        var segments = m.Groups[1].Value.Split('.');
        for (int i = segments.Length; i > 0; i--)
        {
            string candidate = string.Join(".", segments[..i]);
            var known = world.QuireTypes.Keys.FirstOrDefault(k => k == candidate || k.StartsWith(candidate + "`", StringComparison.Ordinal));
            if (known == null) continue;
            if (known.Contains('`')) known = candidate;
            return names.Type(new NamedSig("", string.Join("+", segments[i..].Prepend(known))));
        }
        return names.Type(new NamedSig("", m.Groups[1].Value));
    });

    /// <summary>Where each stub member is declared in the project's generated files: methods by their PDB line,
    /// fields and field-like events by name within their type's block.</summary>
    private Dictionary<Stub, Site> Sites(List<Stub> stubs)
    {
        var lines = Lines();
        var byType = stubs.ToLookup(s => s.Type.FullName);
        var result = new Dictionary<Stub, Site>();
        foreach (string file in Directory.GetFiles(Path.Combine(repo, "src", project, "Generated", "Mirror"), "*.g.cs"))
        {
            var text = File.ReadAllText(file).Split('\n');
            var path = new List<(string Name, string Kind, int Depth)>();
            int depth = 0;
            string? pendingMirror = null;
            for (int i = 0; i < text.Length; i++)
            {
                string t = text[i].TrimEnd('\r').Trim();
                if (t.StartsWith("[global::Quire.Mirror.MirrorOf(")) pendingMirror = t;
                else if (pendingMirror != null && TypeDeclaration().Match(t) is { Success: true } decl)
                {
                    path.Add((decl.Groups["name"].Value, decl.Groups["kind"].Value, depth));
                    pendingMirror = null;
                }
                if (t == Stub && i + 1 < text.Length && (i == 0 || !text[i - 1].Trim().StartsWith('[')))
                {
                    string member = text[i + 1].TrimEnd('\r');
                    var type = quire.Model.Types.FirstOrDefault(x => x.MirrorOf != null && x.FullName == FullName(path));
                    if (type != null)
                    {
                        var candidates = byType[type.FullName];
                        var tokens = lines.GetValueOrDefault((Path.GetFileName(file), i + 2)) ?? [];
                        var stub = candidates.FirstOrDefault(s => s.Methods.Count > 0 && s.Methods.All(p => tokens.Contains(p.Quire.Token)))
                            ?? candidates.FirstOrDefault(s => s.Kind is "field" or "event" && Regex.IsMatch(member, $@"\s@?{Regex.Escape(s.Name)}\s*(;|$|\{{)"));
                        if (stub != null && !result.ContainsKey(stub))
                            result[stub] = new(file, i + 1, member, [.. path.Select(p => p.Name)], [.. path.Select(p => p.Kind)]);
                    }
                }
                depth += t.Count(c => c == '{') - t.Count(c => c == '}');
                while (path.Count > 0 && depth <= path[^1].Depth && t.Contains('}')) path.RemoveAt(path.Count - 1);
            }
        }
        return result;
    }

    private string FullName(List<(string Name, string Kind, int Depth)> path)
    {
        if (path.Count == 0) return "";
        var top = quire.Model.Types.Where(t => t.DeclaringType == null && Strip(t.Name) == path[0].Name);
        string name = top.Select(t => t.FullName).FirstOrDefault() ?? "";
        foreach (var nested in path.Skip(1))
            name = quire.Model.Types.FirstOrDefault(t => t.DeclaringType == name && Strip(t.Name) == nested.Name)?.FullName ?? "";
        return name;
    }

    private static string Strip(string name) => name.Contains('`') ? name[..name.IndexOf('`')] : name;

    [GeneratedRegex(@"\b(?<kind>class|struct|interface|record)\s+@?(?<name>\w+)")]
    private static partial Regex TypeDeclaration();

    /// <summary>(generated file name, line) → method tokens with a sequence point there, from the PDB.</summary>
    private Dictionary<(string, int), HashSet<int>> Lines()
    {
        var map = new Dictionary<(string, int), HashSet<int>>();
        using var provider = MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(Path.ChangeExtension(quire.Model.Path, ".pdb")));
        var pdb = provider.GetMetadataReader();
        foreach (var m in quire.Methods.Where(m => m.BodyRva != 0))
            foreach (var point in pdb.GetMethodDebugInformation(MetadataTokens.MethodDefinitionHandle(m.Token).ToDebugInformationHandle()).GetSequencePoints())
            {
                if (point.IsHidden) continue;
                string doc = pdb.GetString(pdb.GetDocument(point.Document).Name).Replace('\\', '/');
                if (!doc.Contains("/Generated/Mirror/")) continue;
                var key = (Path.GetFileName(doc), point.StartLine);
                if (!map.TryGetValue(key, out var set)) map[key] = set = [];
                set.Add(m.Token);
            }
        return map;
    }
}
