using System.Text.Json;

namespace Quire.Tools.MirrorGen;

/// <summary>Shared source directory resolver for MirrorGen and MethodAudit.</summary>
public static class ProjectLayout
{
    private static Dictionary<string, string> Overrides(string repo)
    {
        string path = Path.Combine(repo, "docs", "mirror", "source-layout.json");
        return File.Exists(path)
            ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path)) ?? []
            : [];
    }

    public static string ProjectDir(string repo, string project)
    {
        string relative = Overrides(repo).GetValueOrDefault(project, project);
        string src = Path.GetFullPath(Path.Combine(repo, "src"));
        string directory = Path.GetFullPath(Path.Combine(src, relative));
        if (!directory.StartsWith(src + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"{project}: source layout must stay under src/: {relative}");
        return directory;
    }

    public static string ProjectFile(string repo, string project, string extension)
    {
        string directory = ProjectDir(repo, project);
        string expected = Path.Combine(directory, project + extension);
        if (File.Exists(expected)) return expected;
        if (!Directory.Exists(directory)) return expected;
        var nested = NestedProjectDirs(repo, project)
            .Select(path => Path.GetFullPath(path) + Path.DirectorySeparatorChar).ToArray();
        return Directory.EnumerateFiles(directory, project + extension, SearchOption.AllDirectories)
            .Where(path => !Path.GetRelativePath(directory, path).Split(Path.DirectorySeparatorChar)
                .Any(part => part is "bin" or "obj"))
            .Where(path => !nested.Any(child => Path.GetFullPath(path).StartsWith(child, StringComparison.OrdinalIgnoreCase)))
            .FirstOrDefault() ?? expected;
    }

    public static IEnumerable<string> Projects(string repo, string extension)
    {
        string src = Path.Combine(repo, "src");
        if (!Directory.Exists(src)) yield break;
        foreach (string path in Directory.EnumerateFiles(src, "*" + extension, SearchOption.AllDirectories))
        {
            if (Path.GetRelativePath(src, path).Split(Path.DirectorySeparatorChar)
                .Any(part => part is "bin" or "obj")) continue;
            yield return Path.GetFileNameWithoutExtension(path);
        }
    }

    public static string? ProjectOf(string repo, string path)
    {
        string full = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(repo, path));
        return Projects(repo, ".csproj").Concat(Projects(repo, ".vcxproj")).Concat(Overrides(repo).Keys)
            .Distinct(StringComparer.Ordinal)
            .Select(name => (Name: name, Dir: ProjectDir(repo, name)))
            .Where(project => full.StartsWith(project.Dir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(project => project.Dir.Length).Select(project => project.Name).FirstOrDefault();
    }

    public static IEnumerable<string> NestedProjectDirs(string repo, string project)
    {
        string directory = ProjectDir(repo, project);
        return Projects(repo, ".csproj").Concat(Projects(repo, ".vcxproj")).Concat(Overrides(repo).Keys)
            .Distinct(StringComparer.Ordinal)
            .Select(name => ProjectDir(repo, name))
            .Where(nested => nested.StartsWith(directory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
