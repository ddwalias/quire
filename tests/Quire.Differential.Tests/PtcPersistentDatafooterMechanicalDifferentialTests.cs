namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.PersistentData.footer</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.PersistentData.footer.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcPersistentDatafooterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000980() => Mechanical.Compare(typeof(global::Quire.PersistentData.footer), 0x06000980);
}
