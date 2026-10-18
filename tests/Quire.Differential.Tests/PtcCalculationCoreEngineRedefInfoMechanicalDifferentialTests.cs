namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.EngineRedefInfo</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.EngineRedefInfo.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreEngineRedefInfoMechanicalDifferentialTests
{
    [SkippableFact]
    public void OldLocation_get_06000049() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineRedefInfo), 0x06000049);

    [SkippableFact]
    public void OldLocation_set_0600004a() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineRedefInfo), 0x0600004a);

    [SkippableFact]
    public void OldType_get_0600004b() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineRedefInfo), 0x0600004b);

    [SkippableFact]
    public void OldType_set_0600004c() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineRedefInfo), 0x0600004c);

    [SkippableFact]
    public void Name_get_0600004d() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineRedefInfo), 0x0600004d);

    [SkippableFact]
    public void Name_set_0600004e() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineRedefInfo), 0x0600004e);
}
