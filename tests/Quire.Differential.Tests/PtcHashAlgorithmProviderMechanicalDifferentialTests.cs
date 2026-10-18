namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.HashAlgorithmProvider</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.HashAlgorithmProvider.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcHashAlgorithmProviderMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetDisposableHash_060001d4() => Mechanical.Compare(typeof(global::Quire.HashAlgorithmProvider), 0x060001d4);

    [SkippableFact]
    public void ctor_060001d8() => Mechanical.Compare(typeof(global::Quire.HashAlgorithmProvider), 0x060001d8);

    [SkippableFact]
    public void IsConnected_get_060001d5() => Mechanical.Compare(typeof(global::Quire.HashAlgorithmProvider), 0x060001d5);

    [SkippableFact]
    public void IsConnected_set_060001d6() => Mechanical.Compare(typeof(global::Quire.HashAlgorithmProvider), 0x060001d6);
}
