using Quire.Tools.MirrorGen;

if (args.Length < 1)
{
    Console.Error.WriteLine("usage: MirrorGen <emit|manifest|native-inventory|scope-diff|collisions|graph|stats|dump <asm> <type>|types <dll> <filter>>");
    return 2;
}

string repo = FindRepoRoot();
string install = Environment.GetEnvironmentVariable("MATHCAD_DIR") ?? @"C:\Program Files\PTC\Mathcad Prime 12.0.1.0";
var rules = args[0] == "types" ? null! : Rules.Load(Path.Combine(repo, "docs", "mirror", "type-map.csv"));

switch (args[0])
{
    case "stats":
        Stats.Run(Universe.LoadMathcad(install, rules), rules);
        return 0;
    case "scope-diff":
        ScopeDiff.Run(Universe.LoadMathcad(install, rules), rules);
        return 0;
    case "dump":
        Dump.Run(Universe.LoadMathcad(install, rules), args[1], args[2]);
        return 0;
    case "types":
        foreach (var t in MetadataLoader.Load(args[1])!.Types.Where(t => t.Access == Access.Public && t.FullName.Contains(args[2], StringComparison.OrdinalIgnoreCase)))
            Console.WriteLine($"{t.FullName} : {t.BaseType}");
        return 0;
    case "emit":
        Emit.Run(repo, Universe.LoadMathcad(install, rules), rules);
        return 0;
    case "native-inventory":
        NativeInventory.Run(install, Path.Combine(repo, "docs", "mirror", "native"));
        return 0;
    case "graph":
    {
        var u = Universe.LoadMathcad(install, rules);
        foreach (var a in u.Assemblies.Values.OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase))
        {
            string stem = Path.GetFileNameWithoutExtension(a.Path);
            var refs = a.References.Where(r => u.IsMathcad(r)).Select(r => Path.GetFileNameWithoutExtension(u.Assemblies[r].Path)).Distinct().OrderBy(r => r);
            Console.WriteLine($"{stem} [{Rules.ProjectFor(stem)}] -> {string.Join(" ", refs)}");
        }
        return 0;
    }
    case "collisions":
    {
        var mapper = new Mapper(Universe.LoadMathcad(install, rules), rules);
        mapper.Build();
        var hw = HandWritten.Load(repo, mapper.Types.Select(t => t.Project!).Distinct());
        var derived = mapper.Types.Where(t => t.BaseType is NamedSig).Select(t => ((NamedSig)t.BaseType!).Name).ToHashSet();
        foreach (var t in mapper.Types.OrderBy(t => t.Project).ThenBy(t => t.FullName))
        {
            if (hw.Find(t) is not { } h) continue;
            var issues = new List<string>();
            if (h.Kind != t.Kind) issues.Add($"kind {h.Kind} vs {t.Kind}");
            if (h.Methods.Any(m => m.Name == "<Clone>$")) issues.Add("record");
            if (h.Access != t.Access) issues.Add($"access {h.Access} vs {t.Access}");
            if (h.IsStatic != t.IsStatic) issues.Add($"static {h.IsStatic} vs {t.IsStatic}");
            if (h.Sealed && !t.Sealed && h.Kind == TypeKind.Class && derived.Contains(t.FullName)) issues.Add("sealed but Mathcad derives from it");
            if (h.Abstract != t.Abstract && h.Kind == TypeKind.Class && !t.IsStatic) issues.Add($"abstract {h.Abstract} vs {t.Abstract}");
            if (h.BaseType is NamedSig hb && hb.Name is not ("System.Object" or "System.ValueType" or "System.Enum" or "System.MulticastDelegate") && t.BaseType?.ToString() != hb.ToString())
                issues.Add($"base {hb} vs {t.BaseType}");
            if (h.GenericParams.Length != t.GenericParams.Length) issues.Add("generic arity");
            Console.WriteLine($"{(issues.Count == 0 ? "ok      " : "CONFLICT")} [{t.Project}] {t.FullName}  {string.Join("; ", issues)}");
        }
        return 0;
    }
    case "count-methods":
        // Non-private methods (accessors included) per type, for cross-checking the reader against other tools.
        foreach (var t in MetadataLoader.Load(args[1])!.Types.OrderBy(t => t.FullName, StringComparer.Ordinal))
            Console.WriteLine($"{t.FullName}\t{Planner.AllMethods(t).Count(m => m.Access != Access.Private)}\t{Planner.AllMethods(t).Count()}");
        return 0;
    case "manifest":
        return Shape.WriteManifestAndCoverage(repo, args.Contains("--accept-removals")) ? 0 : 1;
    default:
        Console.Error.WriteLine($"unknown command {args[0]}");
        return 2;
}

static string FindRepoRoot()
{
    for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        if (File.Exists(Path.Combine(dir.FullName, "Quire.slnx"))) return dir.FullName;
    for (var dir = new DirectoryInfo(Environment.CurrentDirectory); dir != null; dir = dir.Parent)
        if (File.Exists(Path.Combine(dir.FullName, "Quire.slnx"))) return dir.FullName;
    throw new DirectoryNotFoundException("Quire.slnx not found above the tool or the current directory");
}
