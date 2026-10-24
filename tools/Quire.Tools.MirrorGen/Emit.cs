using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Quire.Tools.MirrorGen;

/// <summary>Writes the generated sources, the per-project build settings, new projects and the notes report.</summary>
public static class Emit
{
    public const string GeneratedDir = "Generated";
    public const string MirrorDir = "Mirror";

    public static void Run(string repo, Universe mathcad, Rules rules)
    {
        var mapper = new Mapper(mathcad, rules);
        mapper.Build();
        var externals = Externals.Load();
        var projects = mapper.Types.Select(t => t.Project!).Distinct().OrderBy(p => p, StringComparer.Ordinal).ToList();
        var handWritten = HandWritten.Load(repo, projects);
        var plan = new Planner(mapper, externals, handWritten, Friends(mathcad, rules, projects));
        plan.Run();
        var writer = new CSharpWriter(mapper, plan);
        // D20: a split C++/CLI target is hand-written C++; its types stay in the model only so references resolve.
        var csharp = projects.Where(p => !rules.IsCppProject(p)).ToList();
        // A C++/CLI target that is still another assembly's D2 host would silently lose that assembly's C#
        // (iengine's Quire.Engine hosts engutils-mgd): split the hosted assembly first.
        var stranded = rules.Scope.Values.Select(e => e.Assembly).Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(a => !Rules.IsDeferredAssembly(a) && rules.IsCppProject(rules.HostFor(a)) && rules.HostFor(a) != Rules.ProjectFor(a))
            .ToList();
        if (stranded.Count > 0)
            throw new InvalidOperationException("split these before their C++/CLI host: " +
                string.Join(", ", stranded.Select(a => $"{a} (in {rules.HostFor(a)})")));

        int files = 0, skipped = 0;
        foreach (var project in csharp)
        {
            string dir = Path.Combine(repo, "src", project, GeneratedDir, MirrorDir);
            Directory.CreateDirectory(dir);
            var written = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in mapper.Types.Where(t => t.Project == project && t.DeclaringType == null).OrderBy(t => t.FullName, StringComparer.Ordinal))
            {
                var text = writer.File(t);
                if (text == null) { skipped++; continue; }
                string file = FileName(t.FullName, written);
                WriteIfChanged(Path.Combine(dir, file), text);
                files++;
            }
            foreach (var stale in Directory.GetFiles(dir, "*.g.cs"))
                if (!written.Contains(Path.GetFileName(stale))) File.Delete(stale);
        }

        var deps = Dependencies(repo, mapper, plan, mathcad, rules);
        foreach (var project in csharp) WriteProject(repo, project, deps[project], rules);
        AddToSolution(repo, csharp.Append("Quire.Mirror"), ["tools/Quire.Tools.MirrorGen/Quire.Tools.MirrorGen.csproj"]);
        WriteNotes(repo, mapper, plan);
        UpdateTypeMap(repo, mapper, rules);
        Console.WriteLine($"{mapper.Types.Count} types in {projects.Count} projects; {files} files written, {skipped} types left to hand-written code");
    }

    private static string FileName(string fullName, HashSet<string> taken)
    {
        string name = fullName.StartsWith("Quire.") ? fullName[6..] : fullName;
        name = name.Replace('`', '_');
        if (name.Length > 100)
            name = name[..80] + "_" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(fullName)))[..12];
        string file = name + ".g.cs";
        for (int i = 2; !taken.Add(file); i++) file = $"{name}_{i}.g.cs"; // case-insensitive file systems
        return file;
    }

    private static void WriteIfChanged(string path, string text)
    {
        if (File.Exists(path) && File.ReadAllText(path) == text) return;
        File.WriteAllText(path, text, new UTF8Encoding(false));
    }

    // ------------------------------------------------------------------ dependencies

    private sealed class ProjectDeps
    {
        public SortedSet<string> Projects { get; } = new(StringComparer.Ordinal);
        public SortedDictionary<string, Package> Packages { get; } = new(StringComparer.Ordinal);
        public SortedSet<string> Friends { get; } = new(StringComparer.Ordinal);
        public bool Wpf, WinForms, Unsafe;
        public HashSet<string> NeedsWindowsDesktop { get; } = [];
    }

    private static Dictionary<string, ProjectDeps> Dependencies(string repo, Mapper mapper, Planner plan, Universe mathcad, Rules rules)
    {
        var deps = mapper.Types.Select(t => t.Project!).Distinct().ToDictionary(p => p, _ => new ProjectDeps());
        foreach (var t in mapper.Types)
        {
            if (plan.TypeSkip.ContainsKey(t)) continue;
            var d = deps[t.Project!];
            foreach (var sig in Signatures(t, plan))
                foreach (var n in Stats.Named(sig))
                {
                    if (deps.ContainsKey(n.Assembly) || n.Assembly == "Quire.Core") { if (n.Assembly != t.Project) d.Projects.Add(n.Assembly); continue; }
                    if (Externals.WpfAssemblies.Contains(n.Assembly)) d.Wpf = true;
                    if (Externals.WinFormsAssemblies.Contains(n.Assembly)) d.WinForms = true;
                    if (Externals.Packages.TryGetValue(n.Assembly, out var pkg)) d.Packages[pkg.Id] = pkg;
                    if (Externals.WindowsDesktopPackages.ContainsKey(n.Assembly)) d.NeedsWindowsDesktop.Add(n.Assembly);
                }
            if (Signatures(t, plan).Any(s => s.ToString().Contains('*'))) d.Unsafe = true;
        }

        // Hand-written code may need projects the generated signatures no longer mention (a stub implemented by hand
        // can be the last generated use of another project's type): keep what the built assembly references, the
        // WPF and WinForms frameworks included.
        foreach (var (project, d) in deps)
            foreach (var reference in BuiltReferences(repo, project))
            {
                if (reference != project && deps.ContainsKey(reference)) d.Projects.Add(reference);
                if (Externals.WpfAssemblies.Contains(reference)) d.Wpf = true;
                if (Externals.WinFormsAssemblies.Contains(reference)) d.WinForms = true;
            }

        // InternalsVisibleTo, as Mathcad declares it, mapped to the host projects.
        foreach (var asm in mathcad.Assemblies.Values)
        {
            string project;
            try { project = rules.HostFor(FileStem(asm, mathcad)); }
            catch (ArgumentException) { continue; }
            if (!deps.TryGetValue(project, out var d)) continue;
            foreach (var friend in asm.InternalsVisibleTo)
            {
                try
                {
                    string fp = rules.HostFor(friend);
                    if (fp != project && deps.ContainsKey(fp)) d.Friends.Add(fp);
                }
                catch (ArgumentException) { }
            }
        }

        // Windows-only projects make their dependents Windows-only too.
        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var (p, d) in deps)
                if (!(d.Wpf || d.WinForms) && d.Projects.Any(r => deps.TryGetValue(r, out var rd) && (rd.Wpf || rd.WinForms) || IsWindowsProject(repo, r)))
                { d.Wpf = true; changed = true; }
        }
        foreach (var d in deps.Values)
            if (!d.Wpf && !d.WinForms)
                foreach (var a in d.NeedsWindowsDesktop) d.Packages[Externals.WindowsDesktopPackages[a].Id] = Externals.WindowsDesktopPackages[a];

        DetectCycles(repo, deps.ToDictionary(kv => kv.Key, kv => (IEnumerable<string>)kv.Value.Projects));
        return deps;
    }

    /// <summary>Project → the projects it grants InternalsVisibleTo, as Mathcad's assemblies declare it.</summary>
    private static Dictionary<string, HashSet<string>> Friends(Universe mathcad, Rules rules, IEnumerable<string> projects)
    {
        var known = projects.ToHashSet();
        var result = new Dictionary<string, HashSet<string>>();
        foreach (var asm in mathcad.Assemblies.Values)
        {
            string project;
            try { project = rules.HostFor(Path.GetFileNameWithoutExtension(asm.Path)); }
            catch (ArgumentException) { continue; }
            if (!result.TryGetValue(project, out var set)) result[project] = set = [];
            foreach (var friend in asm.InternalsVisibleTo)
            {
                try { if (known.Contains(rules.HostFor(friend))) set.Add(rules.HostFor(friend)); }
                catch (ArgumentException) { }
            }
        }
        return result;
    }

    private static string FileStem(AssemblyModel asm, Universe u) => Path.GetFileNameWithoutExtension(asm.Path);

    private static IEnumerable<TypeSig> Signatures(TypeModel t, Planner plan)
    {
        if (t.BaseType != null && !plan.DropBase.Contains(t)) yield return t.BaseType;
        foreach (var i in t.Interfaces) yield return i;
        foreach (var g in t.GenericParams) foreach (var c in g.Constraints) yield return c;
        foreach (var f in t.Fields) if (!plan.Skip.Contains(f)) yield return f.Type;
        foreach (var p in t.Properties) if (!plan.Skip.Contains(p)) yield return p.Type;
        foreach (var e in t.Events) if (!plan.Skip.Contains(e)) yield return e.Type;
        foreach (var m in Planner.AllMethods(t))
        {
            if (plan.Skip.Contains(m)) continue;
            yield return m.Return;
            foreach (var p in m.Params) yield return p.Type;
            foreach (var i in m.Implements) yield return i.Interface;
        }
        if (plan.BaseCallFor(t) is { } call) foreach (var a in call.Args) yield return a;
        if (plan.ExtraExplicit.TryGetValue(t, out var extra))
            foreach (var x in extra)
            {
                yield return x.Interface;
                yield return x.Member.Return;
                foreach (var p in x.Member.Params) yield return p.Type;
            }
    }

    private static bool IsWindowsProject(string repo, string project)
    {
        string csproj = Path.Combine(repo, "src", project, project + ".csproj");
        return File.Exists(csproj) && File.ReadAllText(csproj).Contains("-windows");
    }

    private static void DetectCycles(string repo, Dictionary<string, IEnumerable<string>> generated)
    {
        var graph = new Dictionary<string, HashSet<string>>();
        foreach (var (p, refs) in generated) graph[p] = [.. refs];
        foreach (var dir in Directory.GetDirectories(Path.Combine(repo, "src")))
        {
            string name = Path.GetFileName(dir), csproj = Path.Combine(dir, name + ".csproj");
            if (!File.Exists(csproj)) continue;
            if (!graph.TryGetValue(name, out var set)) graph[name] = set = [];
            foreach (Match m in Regex.Matches(File.ReadAllText(csproj), @"ProjectReference Include=""[^""]*\\([^\\""]+)\.csproj"""))
                set.Add(m.Groups[1].Value);
        }
        var state = new Dictionary<string, int>();
        var stack = new List<string>();
        void Visit(string n)
        {
            state[n] = 1; stack.Add(n);
            foreach (var r in graph.GetValueOrDefault(n) ?? [])
            {
                if (state.GetValueOrDefault(r) == 1)
                    throw new InvalidOperationException("project reference cycle: " + string.Join(" → ", stack.SkipWhile(s => s != r).Append(r)));
                if (state.GetValueOrDefault(r) == 0) Visit(r);
            }
            state[n] = 2; stack.RemoveAt(stack.Count - 1);
        }
        foreach (var n in graph.Keys) if (state.GetValueOrDefault(n) == 0) Visit(n);
    }

    // ------------------------------------------------------------------ project files

    /// <summary>Assembly names the project's latest Release build references.</summary>
    private static IEnumerable<string> BuiltReferences(string repo, string project)
    {
        if (Shape.BuiltAssembly(repo, project) is not { } dll) return [];
        using var pe = new System.Reflection.PortableExecutable.PEReader(File.OpenRead(dll));
        var md = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
        return [.. md.AssemblyReferences.Select(h => md.GetString(md.GetAssemblyReference(h).Name))];
    }

    private static void WriteProject(string repo, string project, ProjectDeps d, Rules rules)
    {
        string dir = Path.Combine(repo, "src", project);
        string csproj = Path.Combine(dir, project + ".csproj");
        const string import = @"  <Import Project=""Generated\Mirror.props"" />";
        if (!File.Exists(csproj))
            File.WriteAllText(csproj, $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                  </PropertyGroup>
                {import}
                </Project>

                """);
        else
        {
            string text = File.ReadAllText(csproj);
            if (!text.Contains("Generated\\Mirror.props"))
                File.WriteAllText(csproj, text.Replace("</Project>", import + "\n</Project>"));
        }
        string existing = File.ReadAllText(csproj);

        var sb = new StringBuilder();
        sb.AppendLine("<Project>");
        sb.AppendLine("  <!-- Generated by Quire.Tools.MirrorGen from Mathcad's assembly references. Do not edit. -->");
        sb.AppendLine("  <PropertyGroup>");
        if (d.Wpf || d.WinForms) sb.AppendLine("    <TargetFramework>net10.0-windows</TargetFramework>");
        if (d.Wpf) sb.AppendLine("    <UseWPF>true</UseWPF>");
        if (d.WinForms)
        {
            sb.AppendLine("    <UseWindowsForms>true</UseWindowsForms>");
            // WFO0003: the app's manifest sets DPI awareness, which WinForms would rather configure in code.
            sb.AppendLine("    <NoWarn>$(NoWarn);WFO0003</NoWarn>");
        }
        if (d.Unsafe) sb.AppendLine("    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        if (d.WinForms)
        {
            // Generated code is fully qualified; keep WinForms names out of hand-written WPF code.
            sb.AppendLine(@"    <Using Remove=""System.Windows.Forms"" />");
            sb.AppendLine(@"    <Using Remove=""System.Drawing"" />");
        }
        foreach (var p in d.Projects.Prepend("Quire.Mirror").Distinct())
        {
            // D20: a split C++/CLI project is referenced through its .vcxproj.
            string reference = Path.GetRelativePath(Path.Combine(repo, "src", project), rules.ReferenceFile(p));
            if (!existing.Contains($"\\{p}.csproj\"") && !existing.Contains($"\\{p}.vcxproj\""))
                sb.AppendLine($@"    <ProjectReference Include=""{reference}"" />");
        }
        foreach (var pkg in d.Packages.Values)
            if (!existing.Contains($"\"{pkg.Id}\""))
                sb.AppendLine($@"    <PackageReference Include=""{pkg.Id}"" Version=""{pkg.Version}"" />");
        foreach (var f in d.Friends) sb.AppendLine($@"    <InternalsVisibleTo Include=""{f}"" />");
        sb.AppendLine(@"    <InternalsVisibleTo Include=""$(AssemblyName).Tests"" />");
        sb.AppendLine(@"    <InternalsVisibleTo Include=""Quire.Mirror.Tests"" />");
        sb.AppendLine(@"    <InternalsVisibleTo Include=""Quire.Differential.Tests"" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        WriteIfChanged(Path.Combine(dir, GeneratedDir, "Mirror.props"), sb.ToString());
    }

    private static void AddToSolution(string repo, IEnumerable<string> projects, IEnumerable<string> tools)
    {
        string path = Path.Combine(repo, "Quire.slnx");
        var lines = File.ReadAllLines(path).ToList();
        int srcEnd = lines.FindIndex(l => l.Contains("<Folder Name=\"/src/\">"));
        srcEnd = lines.FindIndex(srcEnd, l => l.Contains("</Folder>"));
        foreach (var p in projects.OrderByDescending(p => p, StringComparer.Ordinal))
        {
            string entry = $"    <Project Path=\"src/{p}/{p}.csproj\" />";
            if (!lines.Any(l => l.Trim() == entry.Trim())) lines.Insert(srcEnd, entry);
        }
        var src = lines.Select((l, i) => (l, i)).Where(x => x.l.Contains("Path=\"src/")).ToList();
        var sorted = src.Select(x => x.l).OrderBy(l => l, StringComparer.Ordinal).ToList();
        for (int i = 0; i < src.Count; i++) lines[src[i].i] = sorted[i];
        if (!lines.Any(l => l.Contains("<Folder Name=\"/tools/\">")))
        {
            int end = lines.FindLastIndex(l => l.Contains("</Solution>"));
            lines.InsertRange(end, ["  <Folder Name=\"/tools/\">", .. tools.Select(t => $"    <Project Path=\"{t}\" />"), "  </Folder>"]);
        }
        File.WriteAllLines(path, lines);
    }

    // ------------------------------------------------------------------ type map

    /// <summary>Writes each mirrored row's final Quire name (as generated) into type-map.csv's quire_type column.</summary>
    private static void UpdateTypeMap(string repo, Mapper mapper, Rules rules)
    {
        string path = Path.Combine(repo, "docs", "mirror", "type-map.csv");
        var lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            var f = Rules.SplitCsv(lines[i]);
            if (!rules.TryFind(Path.GetFileNameWithoutExtension(f[0]), f[1], out var e)) continue;
            if (e.Disposition is not (Disposition.Mirror or Disposition.SharedSource or Disposition.Duplicate or Disposition.OwnEquivalent)) continue;
            f[4] = mapper.QuireNameFor(e);
            lines[i] = string.Join(",", f.Select(Quote));
        }
        File.WriteAllText(path, string.Join("\n", lines) + "\n", new UTF8Encoding(false));
    }

    private static string Quote(string field) =>
        field.IndexOfAny([',', '"', '\n']) < 0 ? field : "\"" + field.Replace("\"", "\"\"") + "\"";

    // ------------------------------------------------------------------ report

    private static void WriteNotes(string repo, Mapper mapper, Planner plan)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Mirror generation notes");
        sb.AppendLine("#");
        sb.AppendLine("# Generated by Quire.Tools.MirrorGen. Each entry is a place where the generated C# differs from Mathcad's");
        sb.AppendLine("# metadata, and why. Entries go away as hand-written code or later phases resolve them.");
        var byReason = new Dictionary<string, int>();
        foreach (var t in mapper.Types.OrderBy(t => t.Project).ThenBy(t => t.FullName, StringComparer.Ordinal))
        {
            if (t.Notes.Count == 0) continue;
            sb.AppendLine();
            sb.AppendLine($"[{t.Project}] {t.FullName}  ({t.MirrorOf})");
            foreach (var n in t.Notes.Distinct())
            {
                sb.AppendLine("  " + n);
                string reason = Regex.Replace(n, @"^(dropped [a-z ]+?)(?: [^:]+)?: ([a-z0-9 :]+?)(?: \(.*| [A-Z<].*)?$", "$1: $2");
                reason = Regex.Replace(reason, @"^(skipped [a-z ]+?) \S+:", "$1:");
                reason = Regex.Replace(reason, @"^(no base member for override) .*", "$1");
                reason = Regex.Replace(reason, @"^(added \d+ explicit interface stub\(s\)).*", "added explicit interface stubs");
                reason = Regex.Replace(reason, @"^(.*declared internal):.*", "$1");
                reason = Regex.Replace(reason, @"^(hand-written base) .*", "$1 differs");
                reason = Regex.Replace(reason, @"^renamed (\w+) .*: name clash$", "renamed $1: name clash (obfuscated names)");
                reason = Regex.Replace(reason, @"^interface accessor .* has no property metadata.*", "accessor without property metadata declared as an explicit property");
                reason = Regex.Replace(reason, @"^(\w+) \w+ not generated: inherited from the hand-written base$", "$1 not generated: inherited from the hand-written base");
                reason = Regex.Replace(reason, @"^hand-written code declares it .*", "hand-written access differs from Mathcad's");
                reason = Regex.Replace(reason, @"^(constructor|method \w+)\(.*\) not generated: .*", "overload covered by a hand-written optional-parameter overload");
                byReason[reason] = byReason.GetValueOrDefault(reason) + 1;
            }
        }
        var header = new StringBuilder();
        header.AppendLine("# Summary (count of notes by kind):");
        foreach (var (r, c) in byReason.OrderByDescending(kv => kv.Value)) header.AppendLine($"#   {c,6}  {r}");
        File.WriteAllText(Path.Combine(repo, "docs", "mirror", "notes.txt"), header + sb.ToString());
    }
}
