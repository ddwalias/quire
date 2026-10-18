namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Text.SubscriptRun</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Text.SubscriptRun.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsTextSubscriptRunMechanicalDifferentialTests
{
    [SkippableFact]
    public void InitBaseline_060000f1() => Mechanical.Compare(typeof(global::Quire.Controls.Text.SubscriptRun), 0x060000f1);

    [SkippableFact]
    public void FontVariantsForCopy_get_060000f2() => Mechanical.Compare(typeof(global::Quire.Controls.Text.SubscriptRun), 0x060000f2);
}
