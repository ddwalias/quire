namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.ResultManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.ResultManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreResultManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void CreateOldResultRefId_0600015f() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.ResultManager), 0x0600015f);

    [SkippableFact]
    public void Instance_get_06000150() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.ResultManager), 0x06000150);

    [SkippableFact]
    public void Results_get_06000164() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.ResultManager), 0x06000164);

    [SkippableFact]
    public void ctor_0600014e() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.ResultManager), 0x0600014e);

    [SkippableFact]
    public void Clear_0600014f() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.ResultManager), 0x0600014f);
}
