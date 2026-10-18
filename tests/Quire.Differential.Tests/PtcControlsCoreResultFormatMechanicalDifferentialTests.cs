namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.ResultFormat</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.ResultFormat.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreResultFormatMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetSchema_0600064b() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x0600064b);

    [SkippableFact]
    public void NumberFormatWasRead_get_06000644() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x06000644);

    [SkippableFact]
    public void NumberFormatWasRead_set_06000643() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x06000643);

    [SkippableFact]
    public void MatrixFormatWasRead_get_06000646() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x06000646);

    [SkippableFact]
    public void MatrixFormatWasRead_set_06000645() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x06000645);

    [SkippableFact]
    public void RootNodeFormatName_get_06000649() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x06000649);

    [SkippableFact]
    public void RootNodeFormatName_set_0600064a() => Mechanical.Compare(typeof(global::Quire.Controls.Core.ResultFormat), 0x0600064a);
}
