namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.TextStyle</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.TextStyle.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcTextStyleMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000327() => Mechanical.Compare(typeof(global::Quire.TextStyle), 0x06000327);

    [SkippableFact]
    public void ctor_06000328() => Mechanical.Compare(typeof(global::Quire.TextStyle), 0x06000328);

    [SkippableFact]
    public void ctor_0600032a() => Mechanical.Compare(typeof(global::Quire.TextStyle), 0x0600032a);

    [SkippableFact]
    public void ctor_0600032b() => Mechanical.Compare(typeof(global::Quire.TextStyle), 0x0600032b);
}
