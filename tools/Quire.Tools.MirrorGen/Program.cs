using Quire.Tools.MirrorGen;

if (args.Length < 1)
{
    Console.Error.WriteLine("usage: MirrorGen <emit|names|manifest|native-inventory|scope-diff|assembly-map|shape <dll>|event-fields <dll>>");
    return 2;
}

// `shape ASSEMBLY`: the canonical shape lines of a built assembly's mirrored types, as the manifest lists them.
if (args is ["shape", var shapeAssembly])
{
    foreach (var line in Shape.ForAssembly(shapeAssembly)) Console.WriteLine(line);
    return 0;
}

// `event-fields ASSEMBLY`: each event whose accessors operate on one backing field (EventBacking), as Type::Event = field.
if (args is ["event-fields", var eventAssembly])
{
    foreach (var type in MetadataLoader.Load(eventAssembly)?.Types ?? [])
        foreach (var e in type.Events.Where(e => e.BackingField != null))
            Console.WriteLine($"{type.FullName}::{e.Name} = {e.BackingField}");
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
    // `names [--assemblies DIR]`: the MirrorName origin check (NameCheck). Reads the Release builds emit reads, or the DLLs
    // in DIR (a gate's build output); writes nothing.
    case "names":
    {
        int at = Array.IndexOf(args, "--assemblies");
        if (at >= 0 && at + 1 < args.Length) Shape.BuildOutputs = Path.GetFullPath(args[at + 1]);
        return NameCheck.Run(repo, Emit.Generate(repo, Universe.LoadMathcad(install, rules), rules)) ? 0 : 1;
    }
    case "native-inventory":
        NativeInventory.Run(install, Path.Combine(repo, "docs", "mirror", "native"));
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
