using System.Reflection;
using System.Runtime.Loader;

namespace Quire.Differential.Tests;

/// <summary>
/// The installed Mathcad Prime, loaded for comparison. Assemblies load into their own context, resolving their
/// dependencies from the install directory, so Mathcad's types never mix with Quire's.
/// </summary>
public static class Mathcad
{
    public static string InstallDir { get; } = Environment.GetEnvironmentVariable("MATHCAD_DIR")
        ?? (OperatingSystem.IsWindows() ? @"C:\Program Files\PTC\Mathcad Prime 12.0.1.0" : "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0");

    public static bool Installed => File.Exists(Path.Combine(InstallDir, "mplrun.dll"));

    private static readonly Lazy<InstallContext> Context = new(() => new InstallContext(InstallDir));

    /// <summary>Skips the calling test when Mathcad isn't installed.</summary>
    public static void Require() => Skip.IfNot(Installed, $"Mathcad Prime is not installed at {InstallDir} (set MATHCAD_DIR)");

    public static Assembly Load(string name)
    {
        Require();
        return Context.Value.LoadFromAssemblyName(new AssemblyName(name));
    }

    public static System.Type Type(string assembly, string fullName) =>
        Load(assembly).GetType(fullName, throwOnError: true)!;

    private sealed class InstallContext(string dir) : AssemblyLoadContext("Mathcad", isCollectible: false)
    {
        protected override Assembly? Load(AssemblyName name)
        {
            string path = Path.Combine(dir, name.Name + ".dll");
            return File.Exists(path) ? LoadFromAssemblyPath(path) : null;
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            string path = Path.Combine(dir, unmanagedDllName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? unmanagedDllName : unmanagedDllName + ".dll");
            return File.Exists(path) ? LoadUnmanagedDllFromPath(path) : 0;
        }
    }
}
