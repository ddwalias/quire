using System.IO;
using Quire.Tools.MirrorGen;

namespace Quire.Mirror.Tests;

/// <summary>
/// Guards the mirrored surface. The manifest in docs/mirror/manifest is the shape MirrorGen generated from Mathcad's
/// metadata; implementing stubs must not change it, and every in-scope Mathcad type must keep its Quire counterpart.
/// </summary>
public class MirrorShapeTests
{
    private static readonly string Repo = FindRepo();

    private static string FindRepo()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "Quire.slnx"))) return dir.FullName;
        throw new DirectoryNotFoundException("Quire.slnx not found above the test output directory");
    }

    public static TheoryData<string> Manifests() =>
        [.. Directory.GetFiles(Path.Combine(Repo, "docs", "mirror", "manifest"), "*.txt").Select(f => Path.GetFileNameWithoutExtension(f)!).Order()];

    [Theory]
    [MemberData(nameof(Manifests))]
    public void Project_keeps_its_mirrored_shape(string project)
    {
        var expected = File.ReadLines(Path.Combine(Repo, "docs", "mirror", "manifest", project + ".txt"))
            .Where(l => l.Length > 0 && !l.StartsWith('#'));
        var actual = Shape.ForAssembly(Path.Combine(AppContext.BaseDirectory, project + ".dll")).ToHashSet();
        var missing = expected.Where(l => !actual.Contains(l)).ToList();
        Assert.True(missing.Count == 0,
            $"{missing.Count} mirrored members of {project} changed or disappeared:\n" + string.Join("\n", missing.Take(40)));
    }

    [Fact]
    public void Every_in_scope_Mathcad_type_has_a_Quire_type()
    {
        var quireTypes = new HashSet<string>();
        foreach (var dll in Directory.GetFiles(AppContext.BaseDirectory, "Quire*.dll"))
            if (MetadataLoader.Load(dll) is { } model)
                foreach (var t in model.Types) quireTypes.Add(t.FullName);

        var missing = new List<string>();
        int inScope = 0;
        foreach (var line in File.ReadLines(Path.Combine(Repo, "docs", "mirror", "type-map.csv")).Skip(1))
        {
            var f = Rules.SplitCsv(line);
            if (f[5] is not ("missing" or "exists" or "exists-elsewhere")) continue;
            inScope++;
            if (!quireTypes.Contains(f[4])) missing.Add($"{f[0]} {f[1]} -> {f[4]}");
        }
        Assert.True(inScope > 5000, $"type map has only {inScope} in-scope rows");
        Assert.True(missing.Count == 0, $"{missing.Count} in-scope Mathcad types have no Quire type:\n" + string.Join("\n", missing.Take(40)));
    }
}
