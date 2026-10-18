namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Properties.Resources</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Properties.Resources.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsPropertiesResourcesMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060001b4() => Mechanical.Compare(typeof(global::Quire.Controls.Properties.Resources), 0x060001b4);

    [SkippableFact]
    public void Culture_get_060001b6() => Mechanical.Compare(typeof(global::Quire.Controls.Properties.Resources), 0x060001b6);

    [SkippableFact]
    public void Culture_set_060001b7() => Mechanical.Compare(typeof(global::Quire.Controls.Properties.Resources), 0x060001b7);
}
