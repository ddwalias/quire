namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.PersistentData.IntegerSizeType</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.PersistentData.IntegerSizeType.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcPersistentDataIntegerSizeTypeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060008e2() => Mechanical.Compare(typeof(global::Quire.PersistentData.IntegerSizeType), 0x060008e2);

    [SkippableFact]
    public void width_get_060008de() => Mechanical.Compare(typeof(global::Quire.PersistentData.IntegerSizeType), 0x060008de);

    [SkippableFact]
    public void width_set_060008df() => Mechanical.Compare(typeof(global::Quire.PersistentData.IntegerSizeType), 0x060008df);

    [SkippableFact]
    public void height_get_060008e0() => Mechanical.Compare(typeof(global::Quire.PersistentData.IntegerSizeType), 0x060008e0);

    [SkippableFact]
    public void height_set_060008e1() => Mechanical.Compare(typeof(global::Quire.PersistentData.IntegerSizeType), 0x060008e1);
}
