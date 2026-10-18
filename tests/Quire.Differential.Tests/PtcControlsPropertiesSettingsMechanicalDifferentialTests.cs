namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Properties.Settings</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Properties.Settings.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsPropertiesSettingsMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060001c2() => Mechanical.Compare(typeof(global::Quire.Controls.Properties.Settings), 0x060001c2);

    [SkippableFact]
    public void Default_get_060001bf() => Mechanical.Compare(typeof(global::Quire.Controls.Properties.Settings), 0x060001bf);
}
