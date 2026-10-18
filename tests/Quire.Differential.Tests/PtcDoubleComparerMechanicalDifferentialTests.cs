namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DoubleComparer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DoubleComparer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDoubleComparerMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000116() => Mechanical.Compare(typeof(global::Quire.DoubleComparer), 0x06000116);

    [SkippableFact]
    public void Epsilon_get_06000113() => Mechanical.Compare(typeof(global::Quire.DoubleComparer), 0x06000113);

    [SkippableFact]
    public void Epsilon_set_06000114() => Mechanical.Compare(typeof(global::Quire.DoubleComparer), 0x06000114);
}
