namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.RegionBorder</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.RegionBorder.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreRegionBorderMechanicalDifferentialTests
{
    [SkippableFact]
    public void HideBusyStateElements_06000610() => Mechanical.Compare(typeof(global::Quire.Controls.Core.RegionBorder), 0x06000610);

    [SkippableFact]
    public void ctor_06000612() => Mechanical.Compare(typeof(global::Quire.Controls.Core.RegionBorder), 0x06000612);
}
