namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.IO.CalculationOverridesHandler</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.IO.CalculationOverridesHandler.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests
{
    [SkippableFact]
    public void SetCalculationOverrides_06000221() => Mechanical.Compare(typeof(global::Quire.Controls.IO.CalculationOverridesHandler), 0x06000221);

    [SkippableFact]
    public void ClearCalculationOverrides_06000222() => Mechanical.Compare(typeof(global::Quire.Controls.IO.CalculationOverridesHandler), 0x06000222);
}
