namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.UserSettingsProvider</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.UserSettingsProvider.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUserSettingsProviderMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetPreviousVersion_06000371() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000371);

    [SkippableFact]
    public void Upgrade_06000373() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000373);

    [SkippableFact]
    public void ctor_06000379() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000379);

    [SkippableFact]
    public void ConfigFolderName_get_06000361() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000361);

    [SkippableFact]
    public void ConfigFolderName_set_06000362() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000362);

    [SkippableFact]
    public void IsConfigCopiedForMOR_get_06000363() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000363);

    [SkippableFact]
    public void IsConfigCopiedForMOR_set_06000364() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000364);

    [SkippableFact]
    public void ApplicationName_get_06000367() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000367);

    [SkippableFact]
    public void ApplicationName_set_06000368() => Mechanical.Compare(typeof(global::Quire.UserSettingsProvider), 0x06000368);
}
