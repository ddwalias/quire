namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.TextStyleData</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.TextStyleData.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcTextStyleDataMechanicalDifferentialTests
{
    [SkippableFact]
    public void ToString_06000341() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x06000341);

    [SkippableFact]
    public void SetStyle_06000344() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x06000344);

    [SkippableFact]
    public void SetStyleName_06000346() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x06000346);

    [SkippableFact]
    public void OldStyle_get_06000342() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x06000342);

    [SkippableFact]
    public void StyleName_get_0600034a() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600034a);

    [SkippableFact]
    public void StyleName_set_0600034b() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600034b);

    [SkippableFact]
    public void StyleKey_get_0600034c() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600034c);

    [SkippableFact]
    public void StyleKey_set_0600034d() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600034d);

    [SkippableFact]
    public void BaseOnKey_get_0600034e() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600034e);

    [SkippableFact]
    public void BaseOnKey_set_0600034f() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600034f);

    [SkippableFact]
    public void Style_get_06000350() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x06000350);

    [SkippableFact]
    public void Style_set_06000351() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x06000351);

    [SkippableFact]
    public void StyleChanged_add_0600033e() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600033e);

    [SkippableFact]
    public void StyleChanged_remove_0600033f() => Mechanical.Compare(typeof(global::Quire.TextStyleData), 0x0600033f);
}
