namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.CalculationOptions</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.CalculationOptions.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCalculationOptionsMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060000c6() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000c6);

    [SkippableFact]
    public void EngineTypeAutoCorrected_get_060000bc() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000bc);

    [SkippableFact]
    public void EngineTypeAutoCorrected_set_060000bd() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000bd);

    [SkippableFact]
    public void CalculationOptionChanged_add_060000c0() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000c0);

    [SkippableFact]
    public void CalculationOptionChanged_remove_060000c1() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000c1);

    [SkippableFact]
    public void BinOctHexModeOptionChanged_add_060000c2() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000c2);

    [SkippableFact]
    public void BinOctHexModeOptionChanged_remove_060000c3() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationOptions), 0x060000c3);
}
