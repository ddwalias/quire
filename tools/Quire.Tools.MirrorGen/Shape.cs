using System.Text;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// The mirrored surface of compiled Quire assemblies as canonical text lines: one per type and one per declared
/// member, private ones included (compiler-generated members excluded). Stub and real members produce the same line, so a stub can be implemented without changing the shape.
/// The checked-in manifest (docs/mirror/manifest) is this output, taken from a build; the shape test recomputes it and
/// fails when a line changes or disappears. It guards against drift; fidelity to Mathcad is the generator's job, and
/// every known difference is listed in docs/mirror/notes.txt.
/// </summary>
public static class Shape
{
    public static IEnumerable<string> Lines(TypeModel t)
    {
        string mods = (t.Abstract && !t.IsStatic ? " abstract" : "") + (t.Sealed && !t.IsStatic && t.Kind == TypeKind.Class ? " sealed" : "")
            + (t.IsStatic ? " static" : "");
        string bases = t.Kind == TypeKind.Class && t.BaseType is NamedSig b && b.Name != "System.Object" ? b.ToString() : "";
        string ifaces = string.Join(", ", t.Interfaces.Select(i => i.ToString()).Order(StringComparer.Ordinal));
        yield return $"T {t.FullName} {t.Kind.ToString().ToLowerInvariant()} {Text(t.Access)}{mods} : {bases} | {ifaces}";
        string prefix = "  " + t.FullName + " ";

        if (t.Kind == TypeKind.Enum)
        {
            foreach (var f in t.Fields.Where(f => f.Const)) yield return $"{prefix}V {f.Name} = {f.Value}";
            yield break;
        }
        if (t.Kind == TypeKind.Delegate)
        {
            if (t.Methods.FirstOrDefault(m => m.Name == "Invoke") is { } invoke) yield return prefix + "D " + Signature(invoke);
            yield break;
        }

        foreach (var f in t.Fields.Where(f => Visible(f.Access, f.Name)))
            yield return $"{prefix}F {Text(f.Access)}{(f.Static && !f.Const ? " static" : "")}{(f.Const ? " const" : "")}{(f.ReadOnly ? " readonly" : "")} {f.Type} {f.Name}";
        foreach (var p in t.Properties)
        {
            var acc = new[] { p.Getter, p.Setter }.OfType<MethodModel>().ToList();
            if (p.Name.Contains('.'))
            {
                foreach (var i in acc.SelectMany(a => a.Implements).Select(i => $"{i.Interface}::{i.Method}").Distinct())
                    yield return $"{prefix}X {i}";
                continue;
            }
            if (!acc.Any(a => Visible(a.Access, a.Name))) continue;
            string index = p.Index.IsEmpty ? "" : "[" + string.Join(",", p.Index.Select(i => i.Type)) + "]";
            string get = p.Getter is { } g && Visible(g.Access, g.Name) ? $" get:{Text(g.Access)}" : "";
            string set = p.Setter is { } s && Visible(s.Access, s.Name) ? $" set:{Text(s.Access)}" : "";
            yield return $"{prefix}P {p.Name}{index} : {p.Type}{(acc[0].Static ? " static" : "")}{Virtuality(acc[0])}{get}{set}";
        }
        foreach (var e in t.Events)
        {
            if (e.Name.Contains('.'))
            {
                foreach (var i in e.Adder?.Implements ?? []) yield return $"{prefix}X {i.Interface}::{i.Method}";
                continue;
            }
            if (e.Adder is { } a && Visible(a.Access, a.Name))
                yield return $"{prefix}E {Text(a.Access)}{(a.Static ? " static" : "")}{Virtuality(a)} {e.Type} {e.Name}";
        }
        foreach (var m in t.Methods)
        {
            if (m.Name == ".cctor" || m.Name.StartsWith('<')) continue;
            if (m.Access == Access.Private && m.Name.Contains('.'))
            {
                foreach (var i in m.Implements) yield return $"{prefix}X {i.Interface}::{i.Method}";
                continue;
            }
            yield return $"{prefix}M {Text(m.Access)}{(m.Static ? " static" : "")}{Virtuality(m)} {Signature(m)}";
        }
    }

    /// <summary>Every declared member except compiler-generated ones (private members are mirrored too).</summary>
    private static bool Visible(Access a, string name) => !name.StartsWith('<');

    private static string Signature(MethodModel m) =>
        $"{m.Name}`{m.GenericParams.Length}({string.Join(",", m.Params.Select(p => (p.Out ? "out " : "") + p.Type))}) -> {m.Return}";

    private static string Virtuality(MethodModel m) =>
        !m.Virtual ? "" : m.Abstract ? " abstract" : !m.NewSlot ? (m.Final ? " sealed-override" : " override") : m.Final ? "" : " virtual";

    private static string Text(Access a) => a switch
    {
        Access.Public => "public", Access.Internal => "internal", Access.Protected => "protected",
        Access.ProtectedInternal => "protected-internal", Access.PrivateProtected => "private-protected", _ => "private",
    };

    /// <summary>Shape lines for every mirrored type ([MirrorOf]) in a compiled assembly.</summary>
    public static List<string> ForAssembly(string dll)
    {
        var model = MetadataLoader.Load(dll) ?? throw new InvalidDataException($"{dll} has no metadata");
        var lines = model.Types.Where(t => t.MirrorOf != null).SelectMany(Lines).ToList();
        lines.Sort(StringComparer.Ordinal);
        return lines;
    }

    // ------------------------------------------------------------------ manifest and coverage

    /// <summary>Writes docs/mirror/manifest/&lt;Project&gt;.txt and docs/mirror/coverage.md from the current build.</summary>
    /// <summary>
    /// Returns false (and writes nothing for that project) when lines of an existing manifest would disappear, unless
    /// <paramref name="acceptRemovals"/>: a regeneration must not silently drop mirrored shape.
    /// </summary>
    public static bool WriteManifestAndCoverage(string repo, bool acceptRemovals)
    {
        bool ok = true;
        string manifestDir = Path.Combine(repo, "docs", "mirror", "manifest");
        Directory.CreateDirectory(manifestDir);
        var rows = new List<(string Project, int Types, int Members, int Stubs)>();
        foreach (var (project, dll) in BuiltProjects(repo))
        {
            var model = MetadataLoader.Load(dll)!;
            var mirrored = model.Types.Where(t => t.MirrorOf != null).ToList();
            if (mirrored.Count == 0) continue;
            var lines = mirrored.SelectMany(Lines).ToList();
            lines.Sort(StringComparer.Ordinal);
            string file = Path.Combine(manifestDir, project + ".txt");
            if (File.Exists(file) && !acceptRemovals)
            {
                var now = lines.ToHashSet();
                var removed = File.ReadLines(file).Where(l => l.Length > 0 && !l.StartsWith('#') && !now.Contains(l)).ToList();
                if (removed.Count > 0)
                {
                    ok = false;
                    Console.Error.WriteLine($"{project}: {removed.Count} manifest lines would disappear (rerun with --accept-removals if intended):");
                    foreach (var r in removed.Take(20)) Console.Error.WriteLine("  - " + r);
                    continue;
                }
            }
            File.WriteAllText(file,
                $"# Mirrored shape of {project}. Generated by MirrorGen manifest; checked by Quire.Mirror.Tests.\n" + string.Join("\n", lines) + "\n",
                new UTF8Encoding(false));
            int members = 0, stubs = 0;
            foreach (var t in mirrored)
            {
                foreach (var m in t.Methods.Where(m => !m.Name.Contains('.') && !m.Name.StartsWith('<') || m.Name == ".ctor")) { members++; if (m.Stub) stubs++; }
                foreach (var p in t.Properties.Where(p => !p.Name.Contains('.'))) { members++; if (p.Stub) stubs++; }
                foreach (var e in t.Events.Where(e => !e.Name.Contains('.'))) { members++; if (e.Stub) stubs++; }
                foreach (var f in t.Fields.Where(f => !f.Name.StartsWith('<') && t.Kind != TypeKind.Enum)) { members++; if (f.Stub) stubs++; }
            }
            rows.Add((project, mirrored.Count, members, stubs));
        }

        var sb = new StringBuilder();
        sb.AppendLine("# Mirror coverage");
        sb.AppendLine();
        sb.AppendLine("Generated by `MirrorGen manifest` from the current build. A member counts as a stub while it carries");
        sb.AppendLine("`[MirrorStub]`; implementing it removes the attribute. Explicit interface implementations and enum values");
        sb.AppendLine("are not counted.");
        sb.AppendLine();
        sb.AppendLine("| Project | Mirrored types | Members | Stubs | Implemented |");
        sb.AppendLine("|---|---:|---:|---:|---:|");
        foreach (var r in rows.OrderBy(r => r.Project, StringComparer.Ordinal))
            sb.AppendLine($"| {r.Project} | {r.Types:N0} | {r.Members:N0} | {r.Stubs:N0} | {Percent(r.Members - r.Stubs, r.Members)} |");
        int tt = rows.Sum(r => r.Types), tm = rows.Sum(r => r.Members), ts = rows.Sum(r => r.Stubs);
        sb.AppendLine($"| **Total** | **{tt:N0}** | **{tm:N0}** | **{ts:N0}** | **{Percent(tm - ts, tm)}** |");
        File.WriteAllText(Path.Combine(repo, "docs", "mirror", "coverage.md"), sb.ToString(), new UTF8Encoding(false));
        Console.WriteLine($"manifest: {rows.Count} projects, {tt} mirrored types, {tm} members, {ts} stubs");
        return ok;
    }

    private static string Percent(int part, int whole) => whole == 0 ? "–" : $"{100.0 * part / whole:0.0}%";

    /// <summary>Each src project's newest Debug build output.</summary>
    public static IEnumerable<(string Project, string Dll)> BuiltProjects(string repo)
    {
        foreach (var dir in Directory.GetDirectories(Path.Combine(repo, "src")).OrderBy(d => d, StringComparer.Ordinal))
        {
            string project = Path.GetFileName(dir), bin = Path.Combine(dir, "bin", "Debug");
            if (!Directory.Exists(bin)) continue;
            var dll = Directory.GetDirectories(bin).Select(d => Path.Combine(d, project + ".dll")).Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
            if (dll != null) yield return (project, dll);
        }
    }
}
