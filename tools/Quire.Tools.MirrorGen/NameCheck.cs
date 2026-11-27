using System.Text.RegularExpressions;

namespace Quire.Tools.MirrorGen;

/// <summary>
/// The origin check for [Quire.Mirror.MirrorName]. MethodAudit trusts the attribute it reads from an assembly, so the
/// attribute must be MirrorGen's and nothing else's: (1) no hand-written source carries one, and (2) each generated
/// file carries exactly the MirrorName claims that emit writes for it, on the same declarations. Only the claims are
/// compared, not the whole file: the committed generated sources are not byte-identical to this emitter's output.
/// The check reads the Release builds emit reads (hand-written members are found there), so run it against the build
/// emit used.
/// </summary>
public static class NameCheck
{
    private static readonly Regex Usage = new(@"\[\s*(?:global::)?(?:Quire\.Mirror\.)?MirrorName\s*\(", RegexOptions.Compiled);
    private static readonly Regex Claim = new(@"^\s*\[(?:global::)?(?:Quire\.Mirror\.)?MirrorName\(""([^""]*)""\)\]\s*$", RegexOptions.Compiled);
    private static readonly Regex Attribute = new(@"^\s*\[.*\]\s*$", RegexOptions.Compiled);

    /// <summary>Runs the check and prints each failure. True when there are none.</summary>
    public static bool Run(string repo, Emit.Generation generation)
    {
        var errors = new List<string>();
        errors.AddRange(HandWrittenUsages(repo));
        // A project with no build in the build directory cannot be compared: its hand-written members are unknown to emit.
        var unbuilt = new List<string>();
        foreach (var project in generation.Csharp)
        {
            if (Shape.BuildOutputs != null && Shape.BuiltAssembly(repo, project) == null) { unbuilt.Add(project); continue; }
            errors.AddRange(GeneratedClaims(repo, generation, project));
        }
        if (unbuilt.Count > 0) Console.WriteLine("names: not checked (no build in the build directory): " + string.Join(", ", unbuilt));
        foreach (var e in errors) Console.Error.WriteLine(e);
        Console.WriteLine(errors.Count == 0 ? "names: every MirrorName is emit's claim" : $"names: {errors.Count} problem(s)");
        return errors.Count == 0;
    }

    /// <summary>MirrorName written outside Generated/ (in any source of the repository's src and tests trees).</summary>
    public static IEnumerable<string> HandWrittenUsages(string repo)
    {
        foreach (var root in new[] { "src", "tests" })
        {
            string dir = Path.Combine(repo, root);
            if (!Directory.Exists(dir)) continue;
            foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
                .Where(f => !Generated(repo, f)))
            {
                var lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                    if (Usage.IsMatch(lines[i]))
                        yield return $"{Rel(repo, file)}:{i + 1}: MirrorName outside Generated/: {lines[i].Trim()}";
            }
        }
    }

    /// <summary>The claims of one project's generated files differ from the claims emit writes for them.</summary>
    public static IEnumerable<string> GeneratedClaims(string repo, Emit.Generation generation, string project)
    {
        string dir = Path.Combine(ProjectLayout.ProjectDir(repo, project), Emit.GeneratedDir, Emit.MirrorDir);
        var (texts, _) = Emit.ProjectFiles(generation, project);
        var onDisk = Directory.Exists(dir)
            ? Directory.GetFiles(dir, "*.g.cs").ToDictionary(f => Path.GetFileName(f)!, f => File.ReadAllText(f), StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in texts.Keys.Union(onDisk.Keys, StringComparer.OrdinalIgnoreCase).OrderBy(f => f, StringComparer.Ordinal))
        {
            var expected = texts.TryGetValue(file, out var text) ? Claims(text) : [];
            var actual = onDisk.TryGetValue(file, out var disk) ? Claims(disk) : [];
            string where = Rel(repo, Path.Combine(dir, file)).Replace('\\', '/');
            foreach (var claim in expected.Except(actual)) yield return $"{where}: emit writes claim missing from the file: {claim}";
            foreach (var claim in actual.Except(expected)) yield return $"{where}: claim not written by emit: {claim}";
        }
    }

    /// <summary>Each MirrorName with the declaration it sits on, as "mathcadName|declaration". A claim is an attribute
    /// line; its declaration is the next line that is not another attribute.</summary>
    public static List<string> Claims(string text)
    {
        var lines = text.Split('\n');
        var claims = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            var m = Claim.Match(lines[i]);
            if (!m.Success) continue;
            int j = i + 1;
            while (j < lines.Length && Attribute.IsMatch(lines[j])) j++;
            string declaration = j < lines.Length ? lines[j].Trim() : "";
            claims.Add($"{m.Groups[1].Value}|{declaration}");
        }
        return claims;
    }

    private static bool Generated(string repo, string file)
    {
        string rel = Rel(repo, file).Replace('\\', '/');
        return rel.Contains("/Generated/") || rel.Contains("/obj/") || rel.Contains("/bin/");
    }

    private static string Rel(string repo, string file) => Path.GetRelativePath(repo, file);
}
