namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.PersistentData.header</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.PersistentData.header.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcPersistentDataheaderMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000977() => Mechanical.Compare(typeof(global::Quire.PersistentData.header), 0x06000977);
}
