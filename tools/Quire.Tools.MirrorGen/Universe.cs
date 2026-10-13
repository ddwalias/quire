namespace Quire.Tools.MirrorGen;

/// <summary>The Mathcad assemblies named in the type map, loaded as metadata.</summary>
public sealed class Universe
{
    /// <summary>MATHCAD_DIR, else the default install path (seen through /mnt/c under WSL).</summary>
    public static string InstallDir => Environment.GetEnvironmentVariable("MATHCAD_DIR")
        ?? (OperatingSystem.IsWindows() ? @"C:\Program Files\PTC\Mathcad Prime 12.0.1.0" : "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0");

    public static bool Installed => File.Exists(Path.Combine(InstallDir, "mplrun.dll"));

    public Dictionary<string, AssemblyModel> Assemblies { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static Universe LoadMathcad(string installDir, Rules rules)
    {
        var u = new Universe();
        foreach (var asm in rules.Scope.Keys.Select(k => k.Asm).Distinct())
        {
            var model = MetadataLoader.Load(Path.Combine(installDir, asm + ".dll"))
                ?? throw new InvalidDataException($"{asm}.dll has no assembly metadata");
            u.Assemblies[model.Name] = model;
        }
        return u;
    }

    public bool IsMathcad(string asm) => Assemblies.ContainsKey(asm);
}
