namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.VCard.VCardFactory</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.VCard.VCardFactory.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcVCardVCardFactoryMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060006b2() => Mechanical.Compare(typeof(global::Quire.VCard.VCardFactory), 0x060006b2);
}
