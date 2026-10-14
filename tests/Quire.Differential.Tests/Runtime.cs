namespace Quire.Differential.Tests;

/// <summary>
/// Both runtimes keep their unit system and dimension scratch space per thread, and only the thread that first
/// touches <c>RuntimeOptions</c> gets them by itself. Tests run on xUnit's threads, so each test syncs default
/// options to its thread on both sides first.
/// </summary>
public static class Runtime
{
    public static void Sync()
    {
        new Quire.Mcd.RuntimeOptions().SyncToThread();
        Reflect.Call(Activator.CreateInstance(Mathcad.Type("mplrun", "Mcd.RuntimeOptions"))!, "SyncToThread");
    }
}
