namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.NumberFormat</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.NumberFormat.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreNumberFormatMechanicalDifferentialTests
{
    [SkippableFact]
    public void IsInValidExponentialThresholdRange_060004bf() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004bf);

    [SkippableFact]
    public void IsInValidZeroThresholdRange_060004c0() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004c0);

    [SkippableFact]
    public void IsInValidComplexThresholdRange_060004c1() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004c1);

    [SkippableFact]
    public void FormatChanged_add_060004b7() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004b7);

    [SkippableFact]
    public void FormatChanged_remove_060004b8() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004b8);

    [SkippableFact]
    public void PropertyChanged_add_060004e0() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004e0);

    [SkippableFact]
    public void PropertyChanged_remove_060004e1() => Mechanical.Compare(typeof(global::Quire.Controls.Core.NumberFormat), 0x060004e1);
}
