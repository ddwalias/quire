namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Text.SuperscriptRun</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Text.SuperscriptRun.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsTextSuperscriptRunMechanicalDifferentialTests
{
    [SkippableFact]
    public void InitBaseline_060000f8() => Mechanical.Compare(typeof(global::Quire.Controls.Text.SuperscriptRun), 0x060000f8);

    [SkippableFact]
    public void FontVariantsForCopy_get_060000f9() => Mechanical.Compare(typeof(global::Quire.Controls.Text.SuperscriptRun), 0x060000f9);
}
