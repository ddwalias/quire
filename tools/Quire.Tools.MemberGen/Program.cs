using System.Reflection.Metadata;
using Quire.Tools.MirrorGen;

// MemberGen reconstructs mechanical [MirrorStub] members (accessors, forwarding calls, constructors that store their
// arguments) from Mathcad's IL; see docs/mirror/tooling.md#membergen.
//   survey --repo DIR --install DIR --output FILE [--project NAME ...]
//   emit   --repo DIR --install DIR --project NAME [--assemblies DIR] [--skip FILE] [--report FILE] [--include-tier-a]
// Both read the Release build (with stubs and PDB) under src/<project>/bin, or the one in --assemblies. `emit` is
// internal to accept.py: it marks what it writes [MirrorVerified] before anything is verified, and accept.py
// keeps only what then passes.
const string Usage = "usage: MemberGen survey|emit --repo DIR --install DIR [--project NAME ...] [--output FILE] [--skip FILE] [--report FILE] [--include-tier-a]";
var options = new Dictionary<string, List<string>>();
bool includeTierA = false;
for (int i = 1; i < args.Length; i++)
{
    if (args[i] == "--include-tier-a") { includeTierA = true; continue; }
    if (!args[i].StartsWith("--") || i + 1 >= args.Length || args[i + 1].StartsWith("--"))
    {
        Console.Error.WriteLine($"{args[i]}: expected an option with a value\n{Usage}");
        return 2;
    }
    if (!options.TryGetValue(args[i], out var list)) options[args[i]] = list = [];
    list.Add(args[++i]);
}
string One(string name) => options.TryGetValue(name, out var v) && v.Count == 1 ? Path.GetFullPath(v[0])
    : throw new ArgumentException($"exactly one {name} is required");
if (args.Length == 0 || args[0] is not ("survey" or "emit"))
{
    Console.Error.WriteLine(Usage);
    return 2;
}
string repo = One("--repo"), install = One("--install");
var projects = options.GetValueOrDefault("--project") ?? [.. Directory.GetFiles(Path.Combine(repo, "docs/mirror/manifest"), "*.txt")
    .Select(f => Path.GetFileNameWithoutExtension(f)).Order()];
// Release only; referenced projects' copies come along in its output.
string Build(string project) => options.ContainsKey("--assemblies") ? Path.Combine(One("--assemblies"), project + ".dll") : Directory.GetFiles(Path.Combine(repo, "src", project, "bin", "Release"), project + ".dll", SearchOption.AllDirectories)
    .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault() ?? throw new FileNotFoundException($"no Release build of {project}");
var builds = projects.ToDictionary(p => p, Build);
foreach (var build in builds.Values)
    if (new Artifact(build).OptimizationsDisabled)
        throw new InvalidDataException("Debug assemblies are not accepted; rebuild with -c Release");
var world = new World(repo, install, builds.Values.SelectMany(b => Directory.GetFiles(Path.GetDirectoryName(b)!, "Quire.*.dll").Append(b))
    .GroupBy(Path.GetFileName).Select(g => g.First()));

// The framework's and the packages' type names, so that a simple type name is used only where it is unambiguous.
string runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;   // shared/Microsoft.NETCore.App/<version>
string sdk = Path.GetDirectoryName(Path.GetDirectoryName(runtime))!;
string nuget = Environment.GetEnvironmentVariable("NUGET_PACKAGES") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
world.Know(builds.Values.Select(b => Path.GetDirectoryName(b)!)
    .Concat(Directory.GetDirectories(sdk).Select(d => Path.Combine(d, Path.GetFileName(runtime))))
    .Concat(Directory.Exists(Path.Combine(nuget, "microsoft.windowsdesktop.app.ref")) ? Directory.GetDirectories(Path.Combine(nuget, "microsoft.windowsdesktop.app.ref"), "net*-windows", SearchOption.AllDirectories) : []));

if (args[0] == "survey")
{
    var rows = new List<(string Project, Plan Plan, string? Rendered)>();
    foreach (var (project, build) in builds)
    {
        var quire = new Artifact(build);
        foreach (var stub in Stubs.Find(quire, world))
        {
            var plan = Stubs.Classify(stub);
            rows.Add((project, plan, plan.Mechanical ? Emitter.RenderFailure(world, quire, plan) : null));
        }
        Console.Error.WriteLine($"{project}: {rows.Count(r => r.Project == project)} stubs");
    }
    File.WriteAllText(One("--output"), Survey.Report(rows));
    return 0;
}

string target = projects.Single();
var skip = options.ContainsKey("--skip") ? File.ReadAllLines(One("--skip")).Where(l => l.Length > 0).ToHashSet() : [];
// D18 assigns these whole assemblies, superseding their historical tier deferrals.
// Other projects retain the existing Tier B default and optional A/A2 scope.
bool assignedAssembly = target is "Quire.Core" or "Quire.Mpl.Emit" or "Quire.Utils" or "Quire.Engine.Utl";
string tiers = Path.Combine(repo, "docs", "mirror", "tiers");
if (!assignedAssembly && !File.Exists(Path.Combine(tiers, "tier-b.txt")))
{
    Console.Error.WriteLine($"{Path.Combine(tiers, "tier-b.txt")} is missing: MemberGen generates only listed Tier B members (and Tier A/A2 with --include-tier-a)");
    return 2;
}
var allowed = assignedAssembly
    ? Stubs.Find(new Artifact(builds[target]), world).SelectMany(s => s.Methods)
        .Select(m => $"{m.Quire.Owner}::{m.Quire.Key}").ToHashSet()
    : (includeTierA ? new[] { "tier-b.txt", "tier-a.txt", "tier-a2.txt" } : ["tier-b.txt"])
        .SelectMany(f => File.ReadAllLines(Path.Combine(tiers, f))).Select(l => l.Trim()).Where(l => l.Length > 0).ToHashSet();
var emitter = new Emitter(repo, world, new Artifact(builds[target]), target, skip, allowed);
var report = emitter.Run();
if (options.ContainsKey("--report")) File.WriteAllText(One("--report"), report);
Console.WriteLine($"MemberGen emit {target}: {emitter.Emitted.Count} members emitted, {emitter.Moved} fields moved");
return 0;
