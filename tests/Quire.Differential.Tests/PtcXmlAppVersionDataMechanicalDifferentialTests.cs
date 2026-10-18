namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Xml.AppVersionData</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Xml.AppVersionData.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcXmlAppVersionDataMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060003ac() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003ac);

    [SkippableFact]
    public void CurrentAppVersion_get_060003a6() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003a6);

    [SkippableFact]
    public void CurrentAppVersion_set_060003a7() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003a7);

    [SkippableFact]
    public void CurrentFormatVersion_get_060003a8() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003a8);

    [SkippableFact]
    public void CurrentFormatVersion_set_060003a9() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003a9);

    [SkippableFact]
    public void CurrentEngNbVersion_get_060003aa() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003aa);

    [SkippableFact]
    public void CurrentEngNbVersion_set_060003ab() => Mechanical.Compare(typeof(global::Quire.Xml.AppVersionData), 0x060003ab);
}
