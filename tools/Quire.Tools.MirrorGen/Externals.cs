namespace Quire.Tools.MirrorGen;

/// <summary>A NuGet package that supplies a dependency or substitute assembly.</summary>
public sealed record Package(string Id, string Version, string LibFolder);

/// <summary>
/// Types outside Mathcad and Quire, read from the .NET reference packs and from the packages Quire depends on. The
/// planner uses them to check overrides, interface implementations and base constructors.
/// </summary>
public sealed class Externals
{
    /// <summary>Assembly name → the package providing it. Assemblies in the .NET and Windows Desktop packs need none.</summary>
    public static readonly Dictionary<string, Package> Packages = new()
    {
        ["Fluent"] = new("Fluent.Ribbon", "11.0.2", "net8.0-windows7.0"),
        ["ControlzEx"] = new("ControlzEx", "7.0.3", "net8.0-windows7.0"),
        ["AvalonDock"] = new("Dirkster.AvalonDock", "5.0.1", "net10.0-windows7.0"),
        ["Microsoft.Xaml.Behaviors"] = new("Microsoft.Xaml.Behaviors.Wpf", "1.1.161", "net10.0-windows7.0"),
        ["JetBrains.Annotations"] = new("JetBrains.Annotations", "2024.3.0", "netstandard2.0"),
        // Mathcad ships 2.17.1; 3.x keeps the type names and namespaces (some types moved to the Framework assembly).
        ["DocumentFormat.OpenXml"] = new("DocumentFormat.OpenXml", "3.5.1", "net8.0"),
        ["DocumentFormat.OpenXml.Framework"] = new("DocumentFormat.OpenXml", "3.5.1", "net8.0"),
        ["Python.Runtime"] = new("pythonnet", "3.0.5", "netstandard2.0"),
        ["ClearScript.Core"] = new("Microsoft.ClearScript.Windows", "7.4.5", "net5.0"),
        ["ClearScript.Windows"] = new("Microsoft.ClearScript.Windows", "7.4.5", "net5.0"),
        ["ClearScript.Windows.Core"] = new("Microsoft.ClearScript.Windows", "7.4.5", "net5.0"),
    };

    public static readonly HashSet<string> WpfAssemblies =
        ["WindowsBase", "PresentationCore", "PresentationFramework", "System.Xaml", "System.Printing", "ReachFramework",
         "PresentationFramework.Aero2", "UIAutomationTypes", "UIAutomationProvider", "Fluent", "ControlzEx", "AvalonDock",
         "Microsoft.Xaml.Behaviors"];

    public static readonly HashSet<string> WinFormsAssemblies = ["System.Windows.Forms", "System.Drawing.Common", "System.Drawing"];

    /// <summary>Packs that come with UseWPF/UseWindowsForms; elsewhere these need a package.</summary>
    public static readonly Dictionary<string, Package> WindowsDesktopPackages = new()
    {
        ["System.Configuration.ConfigurationManager"] = new("System.Configuration.ConfigurationManager", "10.0.0", ""),
        ["System.Drawing.Common"] = new("System.Drawing.Common", "10.0.0", ""),
        ["System.IO.Packaging"] = new("System.IO.Packaging", "10.0.0", ""),
    };

    private readonly Dictionary<string, TypeModel> _byName = [];
    public HashSet<string> LoadedAssemblies { get; } = new(StringComparer.OrdinalIgnoreCase);

    public TypeModel? Find(string fullName) => _byName.GetValueOrDefault(fullName);

    public static Externals Load()
    {
        var e = new Externals();
        string dotnet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "packs");
        foreach (var pack in new[] { "Microsoft.NETCore.App.Ref", "Microsoft.WindowsDesktop.App.Ref" })
        {
            var versions = Directory.GetDirectories(Path.Combine(dotnet, pack))
                .Where(d => Version.TryParse(Path.GetFileName(d), out var v) && v.Major == 10)
                .OrderByDescending(d => Version.Parse(Path.GetFileName(d))).ToList();
            if (versions.Count == 0) throw new DirectoryNotFoundException($"{pack} 10.x not found under {dotnet}");
            e.LoadDirectory(Path.Combine(versions[0], "ref", "net10.0"));
        }
        string nuget = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        foreach (var (asm, p) in Packages)
        {
            string file = Path.Combine(nuget, p.Id.ToLowerInvariant(), p.Version, "lib", p.LibFolder, asm + ".dll");
            if (File.Exists(file)) e.LoadFile(file);
        }
        return e;
    }

    private void LoadDirectory(string dir)
    {
        foreach (var file in Directory.GetFiles(dir, "*.dll")) LoadFile(file);
    }

    private void LoadFile(string file)
    {
        AssemblyModel? model;
        try { model = MetadataLoader.Load(file); }
        catch (BadImageFormatException) { return; }
        if (model == null) return;
        LoadedAssemblies.Add(model.Name);
        foreach (var t in model.Types)
            if (t.Access == Access.Public || t.DeclaringType != null) _byName.TryAdd(t.FullName, t);
    }
}
