namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Services.AppServiceRegistry</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Services.AppServiceRegistry.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcServicesAppServiceRegistryMechanicalDifferentialTests
{
    [SkippableFact]
    public void AddService_060007a4() => Mechanical.Compare(typeof(global::Quire.Services.AppServiceRegistry), 0x060007a4);

    [SkippableFact]
    public void RemoveService_060007a5() => Mechanical.Compare(typeof(global::Quire.Services.AppServiceRegistry), 0x060007a5);

    [SkippableFact]
    public void GetService_060007a6() => Mechanical.Compare(typeof(global::Quire.Services.AppServiceRegistry), 0x060007a6);
}
