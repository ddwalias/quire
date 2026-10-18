namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.EngineResult</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.EngineResult.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreEngineResultMechanicalDifferentialTests
{
    [SkippableFact]
    public void ResultNode_get_06000058() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x06000058);

    [SkippableFact]
    public void ResultNode_set_06000059() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x06000059);

    [SkippableFact]
    public void Errors_get_0600005a() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x0600005a);

    [SkippableFact]
    public void Errors_set_0600005b() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x0600005b);

    [SkippableFact]
    public void Warnings_get_0600005c() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x0600005c);

    [SkippableFact]
    public void Warnings_set_0600005d() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x0600005d);

    [SkippableFact]
    public void SynchronizationStatus_get_0600005e() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x0600005e);

    [SkippableFact]
    public void SynchronizationStatus_set_0600005f() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x0600005f);

    [SkippableFact]
    public void SerializationTag_get_06000062() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x06000062);

    [SkippableFact]
    public void SerializationTag_set_06000063() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EngineResult), 0x06000063);
}
