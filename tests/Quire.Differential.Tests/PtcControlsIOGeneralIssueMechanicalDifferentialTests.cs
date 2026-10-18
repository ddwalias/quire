namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.GeneralIssue</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.GeneralIssue.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOGeneralIssueMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000231() => Mechanical.Compare(typeof(global::Quire.Controls.IO.GeneralIssue), 0x06000231);

    [SkippableFact]
    public void Issue_get_0600022f() => Mechanical.Compare(typeof(global::Quire.Controls.IO.GeneralIssue), 0x0600022f);

    [SkippableFact]
    public void Issue_set_06000230() => Mechanical.Compare(typeof(global::Quire.Controls.IO.GeneralIssue), 0x06000230);
}
