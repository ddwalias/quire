namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.PersistentDataObjects.zAxis</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.PersistentDataObjects.zAxis.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcPersistentDataObjectszAxisMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000cdd() => Mechanical.Compare(typeof(global::Quire.PersistentDataObjects.zAxis), 0x06000cdd);

    [SkippableFact]
    public void zDomain_get_06000cdb() => Mechanical.Compare(typeof(global::Quire.PersistentDataObjects.zAxis), 0x06000cdb);

    [SkippableFact]
    public void zDomain_set_06000cdc() => Mechanical.Compare(typeof(global::Quire.PersistentDataObjects.zAxis), 0x06000cdc);
}
