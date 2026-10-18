namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.QualityAgentManager.QualityAgentManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.QualityAgentManager.QualityAgentManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetMathcadPrimeInstallationDate_06000833() => Mechanical.Compare(typeof(global::Quire.QualityAgentManager.QualityAgentManager), 0x06000833);

    [SkippableFact]
    public void IsReportSendingEnabled_get_06000829() => Mechanical.Compare(typeof(global::Quire.QualityAgentManager.QualityAgentManager), 0x06000829);

    [SkippableFact]
    public void IsReportSendingEnabled_set_06000828() => Mechanical.Compare(typeof(global::Quire.QualityAgentManager.QualityAgentManager), 0x06000828);
}
