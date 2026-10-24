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

        // A field-like event's backing field shares its name; an event with written accessors has none.
        foreach (var f in t.Fields.Where(f => Visible(f.Access, f.Name) && !t.Events.Any(e => e.Name == f.Name)))
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

    /// <summary>Write metadata manifests from production Release assemblies.</summary>
    public static bool WriteManifest(string repo, bool acceptRemovals)
    {
        bool ok = true;
        int projects = 0;
        string manifestDir = Path.Combine(repo, "docs", "mirror", "manifest");
        Directory.CreateDirectory(manifestDir);
        foreach (var (project, dll) in BuiltProjects(repo))
        {
            var lines = ForAssembly(dll);
            if (lines.Count == 0) continue;
            string file = Path.Combine(manifestDir, project + ".txt");
            if (File.Exists(file) && !acceptRemovals)
            {
                var now = lines.ToHashSet();
                var removed = File.ReadLines(file).Where(l => l.Length > 0 && !l.StartsWith('#') && !now.Contains(l)).ToList();
                if (removed.Count > 0)
                {
                    ok = false;
                    Console.Error.WriteLine($"{project}: {removed.Count} manifest lines would disappear; use --accept-removals if intended.");
                    continue;
                }
            }
            File.WriteAllText(file,
                $"# Mirrored shape of {project}. Generated by MirrorGen manifest.\n" + string.Join("\n", lines) + "\n",
                new UTF8Encoding(false));
            projects++;
        }
        Console.WriteLine($"manifest: {projects} projects");
        return ok;
    }

    /// <summary>Each src project's newest Release build output: for a mixed-mode project (D19), the C++/CLI DLL it ships.</summary>
    public static IEnumerable<(string Project, string Dll)> BuiltProjects(string repo)
    {
        foreach (var dir in Directory.GetDirectories(Path.Combine(repo, "src")).OrderBy(d => d, StringComparer.Ordinal))
            if (ReleaseOutput(repo, Path.GetFileName(dir)) is { } dll) yield return (Path.GetFileName(dir), dll);
    }

    private static string? ReleaseOutput(string repo, string project)
    {
        string bin = Path.Combine(repo, "src", project, "bin", "Release");
        return Directory.Exists(bin)
            ? Directory.GetDirectories(bin).Select(d => Path.Combine(d, project + ".dll")).Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault()
            : null;
    }

    /// <summary>
    /// A project's newest Release C# compilation: its generated stubs and hand-written C#. A mixed-mode Release build
    /// (D19) replaces the DLL in bin with the C++/CLI one, whose members come from C++ source; the C# stand-in stays in obj.
    /// </summary>
    public static string? BuiltAssembly(string repo, string project)
    {
        var dll = ReleaseOutput(repo, project);
        if (dll == null || IsILOnly(dll)) return dll;
        string standIn = Path.Combine(repo, "src", project, "obj", "Release", Path.GetFileName(Path.GetDirectoryName(dll))!, project + ".dll");
        return File.Exists(standIn) ? standIn : null;
    }

    private static bool IsILOnly(string dll)
    {
        using var pe = new System.Reflection.PortableExecutable.PEReader(File.OpenRead(dll));
        return pe.PEHeaders.CorHeader is { } cor && (cor.Flags & System.Reflection.PortableExecutable.CorFlags.ILOnly) != 0;
    }
}
