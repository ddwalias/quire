namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.Settings.CalculationSettings</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.Settings.CalculationSettings.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreSettingsCalculationSettingsMechanicalDifferentialTests
{
    [SkippableFact]
    public void OnSchemaValidation_060009ff() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Settings.CalculationSettings), 0x060009ff);

    [SkippableFact]
    public void RepairXDocument_06000a00() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Settings.CalculationSettings), 0x06000a00);

    [SkippableFact]
    public void ShouldValidateOnWrite_get_06000a01() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Settings.CalculationSettings), 0x06000a01);

    [SkippableFact]
    public void ShouldValidateOnRead_get_06000a02() => Mechanical.Compare(typeof(global::Quire.Controls.Core.Settings.CalculationSettings), 0x06000a02);
}
