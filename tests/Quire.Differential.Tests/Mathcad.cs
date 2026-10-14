using System.Reflection;
using System.Security.Cryptography;
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

    private const string ExpectedSha256 = "15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52";
    private static readonly Lazy<bool> Identity = new(() =>
    {
        using var stream = File.OpenRead(Path.Combine(InstallDir, "mplrun.dll"));
        string actual = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
        if (actual != ExpectedSha256)
            throw new HarnessException($"mplrun.dll artifact changed: expected SHA-256 {ExpectedSha256}, got {actual}. Revalidate evidence before updating the identity.");
        return true;
    });

    /// <summary>Checks the recorded artifact; a missing install skips unless strict validation is requested.</summary>
    public static void Require()
    {
        if (!Installed && Environment.GetEnvironmentVariable("QUIRE_REQUIRE_MATHCAD") == "1")
            throw new HarnessException($"Required Mathcad install missing at {InstallDir} (set MATHCAD_DIR)");
        Skip.IfNot(Installed, $"Mathcad Prime is not installed at {InstallDir} (set MATHCAD_DIR)");
        _ = Identity.Value;
    }

    public static Assembly Load(string name)
    {
        Require();
        var assembly = Context.Value.LoadFromAssemblyName(new AssemblyName(name));
        if (name.Equals("mplrun", StringComparison.OrdinalIgnoreCase)
            && assembly.ManifestModule.ModuleVersionId != Guid.Parse("17a0933b-4988-4c18-963d-563998321cc8"))
            throw new HarnessException("Loaded mplrun MVID differs from the recorded artifact.");
        return assembly;
    }

    public static System.Type Type(string assembly, string fullName) =>
        Load(assembly).GetType(fullName) ?? throw new HarnessException($"Original type not found: {assembly}, {fullName}");

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
