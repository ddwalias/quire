namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.ApplicationSettingsSafe</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.ApplicationSettingsSafe.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcApplicationSettingsSafeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000296() => Mechanical.Compare(typeof(global::Quire.ApplicationSettingsSafe), 0x06000296);

    [SkippableFact]
    public void FallbackValues_get_0600028f() => Mechanical.Compare(typeof(global::Quire.ApplicationSettingsSafe), 0x0600028f);
}
