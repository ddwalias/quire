using System.Runtime.CompilerServices;
using Quire.Mcd;
using Quire.Mpl.Internal;

// Mathcad's runtime statics are not thread-safe (Dim.intern adds to a shared Hashtable unlocked, as the original
// does), so test classes must not run in parallel.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Quire.Mpl.Tests;

/// <summary>
/// Dim's type initialiser reads the unit system of whichever thread touches Dim first. Tests run on threads xUnit
/// chooses, so run it once here, on a synced thread, before any test can reach it from an unsynced one.
/// </summary>
internal static class RuntimeSetup
{
    [ModuleInitializer]
    internal static void Initialise()
    {
        RuntimeThread.Ensure();
        RuntimeHelpers.RunClassConstructor(typeof(Dim).TypeHandle);
    }
}
