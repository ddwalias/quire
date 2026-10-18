namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.ShadowCalculatable</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.ShadowCalculatable.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOShadowCalculatableMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertSExpressionToRl_060001c5() => Mechanical.Compare(typeof(global::Quire.Controls.IO.ShadowCalculatable), 0x060001c5);

    [SkippableFact]
    public void ctor_060001c6() => Mechanical.Compare(typeof(global::Quire.Controls.IO.ShadowCalculatable), 0x060001c6);

    [SkippableFact]
    public void ResultsProcessingPriority_get_060001c4() => Mechanical.Compare(typeof(global::Quire.Controls.IO.ShadowCalculatable), 0x060001c4);
}
