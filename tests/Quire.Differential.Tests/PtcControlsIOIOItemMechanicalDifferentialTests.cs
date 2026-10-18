namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.IOItem</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.IOItem.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOIOItemMechanicalDifferentialTests
{
    [SkippableFact]
    public void Region_get_06000246() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItem), 0x06000246);

    [SkippableFact]
    public void IdSexp_get_06000247() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItem), 0x06000247);

    [SkippableFact]
    public void IdName_get_06000248() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItem), 0x06000248);

    [SkippableFact]
    public void IsInput_get_06000249() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItem), 0x06000249);

    [SkippableFact]
    public void PropertyChanged_add_0600024d() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItem), 0x0600024d);

    [SkippableFact]
    public void PropertyChanged_remove_0600024e() => Mechanical.Compare(typeof(global::Quire.Controls.IO.IOItem), 0x0600024e);
}
