namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DevelopmentEnvironmentState</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DevelopmentEnvironmentState.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDevelopmentEnvironmentStateMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearRtStateCache_06000203() => Mechanical.Compare(typeof(global::Quire.DevelopmentEnvironmentState), 0x06000203);

    [SkippableFact]
    public void EnableRtInCache_06000204() => Mechanical.Compare(typeof(global::Quire.DevelopmentEnvironmentState), 0x06000204);

    [SkippableFact]
    public void FelinesRtEnvs_get_060001ff() => Mechanical.Compare(typeof(global::Quire.DevelopmentEnvironmentState), 0x060001ff);

    [SkippableFact]
    public void FelinesRtEnvs_set_06000200() => Mechanical.Compare(typeof(global::Quire.DevelopmentEnvironmentState), 0x06000200);
}
