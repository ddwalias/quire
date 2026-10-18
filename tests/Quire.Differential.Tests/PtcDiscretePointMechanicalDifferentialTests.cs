namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DiscretePoint</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DiscretePoint.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDiscretePointMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000186() => Mechanical.Compare(typeof(global::Quire.DiscretePoint), 0x06000186);
}
