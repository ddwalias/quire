namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Serialization.PtcXmlResolver</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Serialization.PtcXmlResolver.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcSerializationPtcXmlResolverMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060007f4() => Mechanical.Compare(typeof(global::Quire.Serialization.PtcXmlResolver), 0x060007f4);

    [SkippableFact]
    public void Credentials_set_060007f5() => Mechanical.Compare(typeof(global::Quire.Serialization.PtcXmlResolver), 0x060007f5);
}
