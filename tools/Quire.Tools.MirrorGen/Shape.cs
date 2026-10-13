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
    public static IEnumerable<string> Lines(TypeModel t) => Entries(t).Select(e => e.Line);

    /// <summary>
    /// The shape lines with the member each describes. Member is null for the type line, enum values, delegate
    /// signatures and explicit interface implementations; those aren't counted as members.
    /// </summary>
    public static IEnumerable<(string Line, IHasStatus? Member)> Entries(TypeModel t)
    {
        string mods = (t.Abstract && !t.IsStatic ? " abstract" : "") + (t.Sealed && !t.IsStatic && t.Kind == TypeKind.Class ? " sealed" : "")
            + (t.IsStatic ? " static" : "");
        string bases = t.Kind == TypeKind.Class && t.BaseType is NamedSig b && b.Name != "System.Object" ? b.ToString() : "";
        string ifaces = string.Join(", ", t.Interfaces.Select(i => i.ToString()).Order(StringComparer.Ordinal));
        yield return ($"T {t.FullName} {t.Kind.ToString().ToLowerInvariant()} {Text(t.Access)}{mods} : {bases} | {ifaces}", null);
        string prefix = "  " + t.FullName + " ";

        if (t.Kind == TypeKind.Enum)
        {
            foreach (var f in t.Fields.Where(f => f.Const)) yield return ($"{prefix}V {f.Name} = {f.Value}", null);
            yield break;
        }
        if (t.Kind == TypeKind.Delegate)
        {
            if (t.Methods.FirstOrDefault(m => m.Name == "Invoke") is { } invoke) yield return (prefix + "D " + Signature(invoke), null);
            yield break;
        }

        foreach (var f in t.Fields.Where(f => Visible(f.Access, f.Name)))
            yield return ($"{prefix}F {Text(f.Access)}{(f.Static && !f.Const ? " static" : "")}{(f.Const ? " const" : "")}{(f.ReadOnly ? " readonly" : "")} {f.Type} {f.Name}", f);
        foreach (var p in t.Properties)
        {
            var acc = new[] { p.Getter, p.Setter }.OfType<MethodModel>().ToList();
            if (p.Name.Contains('.'))
            {
                foreach (var i in acc.SelectMany(a => a.Implements).Select(i => $"{i.Interface}::{i.Method}").Distinct())
                    yield return ($"{prefix}X {i}", null);
                continue;
            }
            if (!acc.Any(a => Visible(a.Access, a.Name))) continue;
            string index = p.Index.IsEmpty ? "" : "[" + string.Join(",", p.Index.Select(i => i.Type)) + "]";
            string get = p.Getter is { } g && Visible(g.Access, g.Name) ? $" get:{Text(g.Access)}" : "";
            string set = p.Setter is { } s && Visible(s.Access, s.Name) ? $" set:{Text(s.Access)}" : "";
            yield return ($"{prefix}P {p.Name}{index} : {p.Type}{(acc[0].Static ? " static" : "")}{Virtuality(acc[0])}{get}{set}", p);
        }
        foreach (var e in t.Events)
        {
            if (e.Name.Contains('.'))
            {
                foreach (var i in e.Adder?.Implements ?? []) yield return ($"{prefix}X {i.Interface}::{i.Method}", null);
                continue;
            }
            if (e.Adder is { } a && Visible(a.Access, a.Name))
                yield return ($"{prefix}E {Text(a.Access)}{(a.Static ? " static" : "")}{Virtuality(a)} {e.Type} {e.Name}", e);
        }
        foreach (var m in t.Methods)
        {
            if (m.Name == ".cctor" || m.Name.StartsWith('<')) continue;
            if (m.Access == Access.Private && m.Name.Contains('.'))
            {
                foreach (var i in m.Implements) yield return ($"{prefix}X {i.Interface}::{i.Method}", null);
                continue;
            }
            yield return ($"{prefix}M {Text(m.Access)}{(m.Static ? " static" : "")}{Virtuality(m)} {Signature(m)}", m);
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

    // ------------------------------------------------------------------ manifest, coverage and status

    private static readonly MemberStatus[] Statuses =
        [MemberStatus.Stub, MemberStatus.Blocked, MemberStatus.Verified, MemberStatus.Documented, MemberStatus.Unclassified, MemberStatus.QuireOnly];

    private static string Text(MemberStatus s) => s switch
    {
        MemberStatus.QuireOnly => "quire-only",
        _ => s.ToString().ToLowerInvariant(),
    };

    /// <summary>
    /// Writes docs/mirror/manifest/&lt;Project&gt;.txt, docs/mirror/status/&lt;Project&gt;.txt and docs/mirror/coverage.md
    /// from the current build. Returns false (and writes nothing for that project) when lines of an existing manifest
    /// would disappear, unless <paramref name="acceptRemovals"/>: a regeneration must not silently drop mirrored shape.
    /// </summary>
    public static bool WriteManifestAndCoverage(string repo, bool acceptRemovals)
    {
        bool ok = true;
        string manifestDir = Path.Combine(repo, "docs", "mirror", "manifest");
        string statusDir = Path.Combine(repo, "docs", "mirror", "status");
        Directory.CreateDirectory(manifestDir);
        Directory.CreateDirectory(statusDir);
        var rows = new List<(string Project, int Types, Dictionary<MemberStatus, int> Counts)>();
        foreach (var (project, dll) in BuiltProjects(repo))
        {
            var model = MetadataLoader.Load(dll)!;
            var mirrored = model.Types.Where(t => t.MirrorOf != null).OrderBy(t => t.FullName, StringComparer.Ordinal).ToList();
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

            var counts = Statuses.ToDictionary(s => s, _ => 0);
            var status = new StringBuilder();
            status.Append($"# Per-member status of {project} (PLAN.md §3c). Generated by MirrorGen manifest.\n");
            status.Append("# Each type with a non-stub member: its counts, then every member that isn't a stub, with the reason,\n");
            status.Append("# test class or source its attribute gives. Stubs are listed in ../manifest.\n");
            foreach (var t in mirrored)
            {
                var members = Entries(t).Where(e => e.Member != null).ToList();
                foreach (var (_, m) in members) counts[m!.Status]++;
                if (members.All(e => e.Member!.Stub)) continue;
                status.Append($"\nT {t.FullName}  {members.Count} members: "
                    + string.Join(", ", Statuses.Select(s => (s, n: members.Count(e => e.Member!.Status == s))).Where(x => x.n > 0).Select(x => $"{Text(x.s)} {x.n}"))
                    + "\n");
                foreach (var (line, m) in members.Where(e => !e.Member!.Stub).OrderBy(e => e.Member!.Status).ThenBy(e => e.Line, StringComparer.Ordinal))
                    status.Append($"  {Text(m!.Status),-12} {line.TrimStart()[(t.FullName.Length + 1)..]}{(m.Evidence is { } ev ? "  [" + ev + "]" : "")}\n");
            }
            File.WriteAllText(Path.Combine(statusDir, project + ".txt"), status.ToString(), new UTF8Encoding(false));
            rows.Add((project, mirrored.Count, counts));
        }

        var sb = new StringBuilder();
        sb.AppendLine("# Mirror coverage");
        sb.AppendLine();
        sb.AppendLine("Generated by `MirrorGen manifest` from the current build. Each member of a mirrored type has one status");
        sb.AppendLine("(PLAN.md §3c), from its attributes:");
        sb.AppendLine();
        sb.AppendLine("- **Stub** `[MirrorStub]`: generated, throws.");
        sb.AppendLine("- **Blocked** `[MirrorBlocked(reason)]`: behaviour can't be established yet; throws, reason recorded.");
        sb.AppendLine("- **Verified** `[MirrorVerified(test)]`: confirmed by a differential test against the installed Mathcad.");
        sb.AppendLine("- **Documented** `[MirrorDocumented(source)]`: implemented from documentation or declared metadata.");
        sb.AppendLine("- **Unclassified** (no attribute): hand-written before §3c; still to be verified, documented, blocked or marked.");
        sb.AppendLine("- **Quire-only** `[QuireOnly]`: no Mathcad counterpart.");
        sb.AppendLine();
        sb.AppendLine("*Settled* is Blocked + Verified + Documented + Quire-only: a phase is done when all its members are settled.");
        sb.AppendLine("Explicit interface implementations, enum values and delegate signatures are not counted. Per-member detail is");
        sb.AppendLine("in [`status/`](status/).");
        sb.AppendLine();
        sb.AppendLine("| Project | Types | Members | Stub | Blocked | Verified | Documented | Unclassified | Quire-only | Settled |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        void Row(string name, int types, Dictionary<MemberStatus, int> c, string b)
        {
            int members = c.Values.Sum();
            int settled = c[MemberStatus.Blocked] + c[MemberStatus.Verified] + c[MemberStatus.Documented] + c[MemberStatus.QuireOnly];
            sb.AppendLine($"| {b}{name}{b} | {b}{types:N0}{b} | {b}{members:N0}{b} | "
                + string.Join(" | ", Statuses.Select(s => $"{b}{c[s]:N0}{b}")) + $" | {b}{Percent(settled, members)}{b} |");
        }
        foreach (var r in rows.OrderBy(r => r.Project, StringComparer.Ordinal)) Row(r.Project, r.Types, r.Counts, "");
        var total = Statuses.ToDictionary(s => s, s => rows.Sum(r => r.Counts[s]));
        Row("Total", rows.Sum(r => r.Types), total, "**");
        File.WriteAllText(Path.Combine(repo, "docs", "mirror", "coverage.md"), sb.ToString(), new UTF8Encoding(false));
        Console.WriteLine($"manifest: {rows.Count} projects, {rows.Sum(r => r.Types)} mirrored types, {total.Values.Sum()} members, "
            + string.Join(", ", Statuses.Select(s => $"{total[s]} {Text(s)}")));
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
