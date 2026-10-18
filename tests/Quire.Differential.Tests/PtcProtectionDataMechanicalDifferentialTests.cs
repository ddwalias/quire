namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.ProtectionData</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.ProtectionData.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcProtectionDataMechanicalDifferentialTests
{
    [SkippableFact]
    public void HasPassword_get_0600024c() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x0600024c);

    [SkippableFact]
    public void Timestamp_get_0600024d() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x0600024d);

    [SkippableFact]
    public void ShowTimestamp_get_0600024e() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x0600024e);

    [SkippableFact]
    public void ShowTimestamp_set_0600024f() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x0600024f);

    [SkippableFact]
    public void ProtectionLevel_get_06000250() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x06000250);

    [SkippableFact]
    public void EncryptedPassword_get_06000255() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x06000255);

    [SkippableFact]
    public void PropertyChanged_add_06000256() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x06000256);

    [SkippableFact]
    public void PropertyChanged_remove_06000257() => Mechanical.Compare(typeof(global::Quire.ProtectionData), 0x06000257);
}
