using Quire.Mcd;

namespace Quire.Mpl.Internal;

/// <summary>
/// The runtime keeps its options (the unit system included) and dimension scratch space per thread, and only the
/// thread that initialises <see cref="RuntimeOptions"/> gets them automatically. Quire code that works with
/// dimensions on other threads calls <see cref="Ensure"/> first.
/// </summary>
internal static class RuntimeThread
{
    /// <summary>Gives the calling thread default runtime options unless it already has some.</summary>
    public static void Ensure()
    {
        if (RuntimeOptions.the_unit_system == null) new RuntimeOptions().SyncToThread();
    }
}
