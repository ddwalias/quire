using System.Reflection.Metadata;
using Microsoft.VisualBasic.FileIO;
using Quire.Tools.MirrorGen;

/// <summary>
/// Both sides' types: the reviewed type-map bindings, Mathcad's assemblies (read on demand from the install) and
/// the built Quire assemblies, so a Mathcad type, field or method can be named as Quire declares it.
/// </summary>
sealed class World
{
    private readonly string _install;
    /// <summary>Quire full name → (Mathcad full name, Mathcad assembly), from docs/mirror/type-map.csv.</summary>
    public Dictionary<string, (string Name, string Assembly)> Bindings { get; } = [];
    private readonly Dictionary<(string Assembly, string Name), string> _quireNames = [];
    private readonly Dictionary<string, Artifact?> _mathcad = new(StringComparer.OrdinalIgnoreCase);
    /// <summary>Every type of the loaded Quire assemblies, by full name.</summary>
    public Dictionary<string, TypeModel> QuireTypes { get; } = [];
    public Dictionary<string, TypeModel> MathcadTypes { get; } = [];
    /// <summary>Public type names of the framework and of every assembly beside the builds, so that a simple name
    /// is used only where no imported namespace declares the same one.</summary>
    public HashSet<string> KnownTypes { get; } = [];

    public World(string repo, string install, IEnumerable<string> quireAssemblies)
    {
        _install = install;
        using (var csv = new TextFieldParser(Path.Combine(repo, "docs/mirror/type-map.csv")))
        {
            csv.SetDelimiters(",");
            csv.HasFieldsEnclosedInQuotes = true;
            csv.ReadFields();
            while (!csv.EndOfData)
            {
                var fields = csv.ReadFields()!;
                if (fields[5] is not ("missing" or "exists" or "exists-elsewhere")) continue;
                string assembly = Path.GetFileNameWithoutExtension(fields[0]);
                Bindings[fields[4]] = (fields[1], assembly);
                _quireNames[(assembly.ToLowerInvariant(), fields[1])] = fields[4];
            }
        }
        foreach (string path in quireAssemblies)
            if (MetadataLoader.Load(path) is { } model)
                foreach (var t in model.Types) QuireTypes.TryAdd(t.FullName, t);
        KnownTypes.UnionWith(QuireTypes.Keys.Select(Unqualified));
    }

    /// <summary>A type's name as a namespace declares it: nested types under their outermost type, no arity.</summary>
    private static string Unqualified(string name) => name.Split('+')[0].Split('`')[0];

    /// <summary>Adds the public type names of the assemblies in <paramref name="directories"/>.</summary>
    public void Know(IEnumerable<string> directories)
    {
        foreach (string file in directories.Where(Directory.Exists).SelectMany(d => Directory.GetFiles(d, "*.dll")).Distinct())
        {
            try
            {
                using var pe = new System.Reflection.PortableExecutable.PEReader(File.OpenRead(file));
                if (!pe.HasMetadata) continue;
                var md = pe.GetMetadataReader();
                foreach (var h in md.TypeDefinitions)
                    if ((md.GetTypeDefinition(h).Attributes & System.Reflection.TypeAttributes.VisibilityMask) is System.Reflection.TypeAttributes.Public
                        or System.Reflection.TypeAttributes.NestedPublic)
                        KnownTypes.Add(Unqualified(SigProvider.FullName(md, h)));
                foreach (var h in md.ExportedTypes)
                {
                    var e = md.GetExportedType(h);
                    string ns = md.GetString(e.Namespace);
                    KnownTypes.Add(Unqualified(ns.Length == 0 ? md.GetString(e.Name) : ns + "." + md.GetString(e.Name)));
                }
            }
            catch (BadImageFormatException) { }
        }
    }

    /// <summary>A Mathcad assembly by simple name; null when it is not part of the install.</summary>
    public Artifact? Mathcad(string assembly)
    {
        if (_mathcad.TryGetValue(assembly, out var artifact)) return artifact;
        string path = Path.Combine(_install, assembly + ".dll");
        artifact = null;
        try { if (File.Exists(path)) artifact = new Artifact(path); } catch (BadImageFormatException) { }
        if (artifact != null)
            foreach (var t in artifact.Model.Types) MathcadTypes.TryAdd(artifact.Name.ToLowerInvariant() + ":" + t.FullName, t);
        return _mathcad[assembly] = artifact;
    }

    public TypeModel? MathcadType(string assembly, string name) =>
        Mathcad(assembly) == null ? null : MathcadTypes.GetValueOrDefault(assembly.ToLowerInvariant() + ":" + name);

    /// <summary>Quire's name for a Mathcad type: its binding, or its outermost bound declaring type's.</summary>
    public string? QuireName(string assembly, string name)
    {
        if (_quireNames.TryGetValue((assembly.ToLowerInvariant(), name), out var quire)) return quire;
        int nest = name.IndexOf('+');
        return nest > 0 && _quireNames.TryGetValue((assembly.ToLowerInvariant(), name[..nest]), out var outer)
            ? outer + name[nest..] : null;
    }

    /// <summary>Mathcad's signature in Quire's names; system types stay as they are.</summary>
    public TypeSig ToQuire(TypeSig sig) => sig switch
    {
        NamedSig n => n with { Name = QuireName(n.Assembly, n.Name) ?? n.Name, Args = [.. n.Args.Select(ToQuire)] },
        ArraySig a => a with { Element = ToQuire(a.Element) },
        ByRefSig b => b with { Element = ToQuire(b.Element) },
        PointerSig p => p with { Element = ToQuire(p.Element) },
        _ => sig,
    };

    /// <summary>Quire's signature in Mathcad's names (MethodAudit's mapping), for matching method keys.</summary>
    public TypeSig ToMathcad(TypeSig sig) => sig switch
    {
        NamedSig n => Bindings.TryGetValue(n.Name, out var b)
            ? n with { Name = b.Name, Assembly = b.Assembly, Args = [.. n.Args.Select(ToMathcad)] }
            : n with { Args = [.. n.Args.Select(ToMathcad)] },
        ArraySig a => a with { Element = ToMathcad(a.Element) },
        ByRefSig b => b with { Element = ToMathcad(b.Element) },
        PointerSig p => p with { Element = ToMathcad(p.Element) },
        _ => sig,
    };

    /// <summary>Whether a Mathcad type is an enum, as far as the install tells.</summary>
    public TypeModel? Enum(TypeSig sig) =>
        sig is NamedSig n && MathcadType(n.Assembly, n.Name) is { Kind: TypeKind.Enum } e ? e : null;
}
