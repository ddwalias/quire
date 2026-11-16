using Quire.Tools.MirrorGen;

if (args.Length < 1)
{
    Console.Error.WriteLine("usage: MirrorGen <emit|manifest|native-inventory|scope-diff|assembly-map|count-methods <dll>|shape <dll>>");
    return 2;
}

// `shape ASSEMBLY`: the canonical shape lines of a built assembly's mirrored types, as the manifest lists them.
if (args is ["shape", var shapeAssembly])
{
    foreach (var line in Shape.ForAssembly(shapeAssembly)) Console.WriteLine(line);
    return 0;
}

string repo = FindRepoRoot();
string install = Universe.InstallDir;
var rules = Rules.Load(repo);

switch (args[0])
{
    case "assembly-map":
        AssemblyMap.Write(repo, install, rules);
        return 0;
    case "scope-diff":
        ScopeDiff.Run(Universe.LoadMathcad(install, rules), rules);
        return 0;
    case "emit":
        Emit.Run(repo, Universe.LoadMathcad(install, rules), rules);
        return 0;
    case "native-inventory":
        NativeInventory.Run(install, Path.Combine(repo, "docs", "mirror", "native"));
        return 0;
    case "count-methods":
        // Non-private methods (accessors included) per type, for cross-checking the reader against other tools.
        foreach (var t in MetadataLoader.Load(args[1])!.Types.OrderBy(t => t.FullName, StringComparer.Ordinal))
            Console.WriteLine($"{t.FullName}\t{Planner.AllMethods(t).Count(m => m.Access != Access.Private)}\t{Planner.AllMethods(t).Count()}");
        return 0;
    case "manifest":
        return Shape.WriteManifest(repo, args.Contains("--accept-removals")) ? 0 : 1;
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
