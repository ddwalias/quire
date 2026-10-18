namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Services.ApplicationVariant</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Services.ApplicationVariant.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcServicesApplicationVariantMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000755() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationVariant), 0x06000755);

    [SkippableFact]
    public void CurrentApplicationType_get_0600074d() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationVariant), 0x0600074d);

    [SkippableFact]
    public void CurrentApplicationType_set_0600074e() => Mechanical.Compare(typeof(global::Quire.Services.ApplicationVariant), 0x0600074e);
}
