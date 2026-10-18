namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Services.MouseFacade</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Services.MouseFacade.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcServicesMouseFacadeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearOverrideCursor_06000794() => Mechanical.Compare(typeof(global::Quire.Services.MouseFacade), 0x06000794);

    [SkippableFact]
    public void GetOverrideCursor_06000795() => Mechanical.Compare(typeof(global::Quire.Services.MouseFacade), 0x06000795);

    [SkippableFact]
    public void SetCursor_06000797() => Mechanical.Compare(typeof(global::Quire.Services.MouseFacade), 0x06000797);

    [SkippableFact]
    public void ctor_0600079a() => Mechanical.Compare(typeof(global::Quire.Services.MouseFacade), 0x0600079a);

    [SkippableFact]
    public void IsConnected_get_06000798() => Mechanical.Compare(typeof(global::Quire.Services.MouseFacade), 0x06000798);

    [SkippableFact]
    public void IsConnected_set_06000799() => Mechanical.Compare(typeof(global::Quire.Services.MouseFacade), 0x06000799);
}
