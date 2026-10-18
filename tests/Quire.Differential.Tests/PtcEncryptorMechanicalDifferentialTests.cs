namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Encryptor</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Encryptor.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcEncryptorMechanicalDifferentialTests
{
    [SkippableFact]
    public void Encrypt_060001c4() => Mechanical.Compare(typeof(global::Quire.Encryptor), 0x060001c4);

    [SkippableFact]
    public void IsConnected_get_060001cb() => Mechanical.Compare(typeof(global::Quire.Encryptor), 0x060001cb);

    [SkippableFact]
    public void IsConnected_set_060001cc() => Mechanical.Compare(typeof(global::Quire.Encryptor), 0x060001cc);
}
