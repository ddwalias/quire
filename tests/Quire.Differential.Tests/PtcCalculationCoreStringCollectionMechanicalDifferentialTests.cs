namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.StringCollection</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.StringCollection.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreStringCollectionMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000014() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.StringCollection), 0x06000014);

    [SkippableFact]
    public void ctor_06000015() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.StringCollection), 0x06000015);
}
