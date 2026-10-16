namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.Initializer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.Initializer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunInitializerMechanicalDifferentialTests
{
    [SkippableFact]
    public void InitializeAssemblyPath_060003b0() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Initializer), 0x060003b0);

    [SkippableFact]
    public void ctor_060003b1() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Initializer), 0x060003b1);
}
