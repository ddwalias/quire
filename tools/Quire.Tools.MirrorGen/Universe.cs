namespace Quire.Tools.MirrorGen;

/// <summary>The Mathcad assemblies named in the type map, loaded as metadata.</summary>
public sealed class Universe
{
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
