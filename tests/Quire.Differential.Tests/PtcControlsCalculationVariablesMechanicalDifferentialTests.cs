namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.CalculationVariables</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.CalculationVariables.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCalculationVariablesMechanicalDifferentialTests
{
    [SkippableFact]
    public void CalculationVariableChanged_add_060000d7() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationVariables), 0x060000d7);

    [SkippableFact]
    public void CalculationVariableChanged_remove_060000d8() => Mechanical.Compare(typeof(global::Quire.Controls.CalculationVariables), 0x060000d8);
}
