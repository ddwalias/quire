namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.IO.UrlUtils</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.IO.UrlUtils.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcIOUrlUtilsMechanicalDifferentialTests
{
    [SkippableFact]
    public void Contains_060013a9() => Mechanical.Compare(typeof(global::Quire.IO.UrlUtils), 0x060013a9);

    [SkippableFact]
    public void ctor_060013ab() => Mechanical.Compare(typeof(global::Quire.IO.UrlUtils), 0x060013ab);

    [SkippableFact]
    public void MatchPattern_get_060013ac() => Mechanical.Compare(typeof(global::Quire.IO.UrlUtils), 0x060013ac);

    [SkippableFact]
    public void SchemeWithDelimiter_get_060013ad() => Mechanical.Compare(typeof(global::Quire.IO.UrlUtils), 0x060013ad);
}
