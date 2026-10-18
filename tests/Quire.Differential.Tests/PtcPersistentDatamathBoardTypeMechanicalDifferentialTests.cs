namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.PersistentData.mathBoardType</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.PersistentData.mathBoardType.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcPersistentDatamathBoardTypeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000904() => Mechanical.Compare(typeof(global::Quire.PersistentData.mathBoardType), 0x06000904);

    [SkippableFact]
    public void disablecalc_get_06000900() => Mechanical.Compare(typeof(global::Quire.PersistentData.mathBoardType), 0x06000900);

    [SkippableFact]
    public void disablecalc_set_06000901() => Mechanical.Compare(typeof(global::Quire.PersistentData.mathBoardType), 0x06000901);

    [SkippableFact]
    public void IsCalculationDisabled_get_06000902() => Mechanical.Compare(typeof(global::Quire.PersistentData.mathBoardType), 0x06000902);

    [SkippableFact]
    public void IsCalculationDisabled_set_06000903() => Mechanical.Compare(typeof(global::Quire.PersistentData.mathBoardType), 0x06000903);
}
