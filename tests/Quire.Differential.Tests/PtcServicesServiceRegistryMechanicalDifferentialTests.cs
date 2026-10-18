namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Services.ServiceRegistry</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Services.ServiceRegistry.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcServicesServiceRegistryMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_0600079e() => Mechanical.Compare(typeof(global::Quire.Services.ServiceRegistry), 0x0600079e);
}
