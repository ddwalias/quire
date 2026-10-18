namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.RegionCollection</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.RegionCollection.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsRegionCollectionMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060000e8() => Mechanical.Compare(typeof(global::Quire.Controls.RegionCollection), 0x060000e8);
}
