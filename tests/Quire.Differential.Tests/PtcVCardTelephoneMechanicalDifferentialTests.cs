namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.VCard.Telephone</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.VCard.Telephone.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcVCardTelephoneMechanicalDifferentialTests
{
    [SkippableFact]
    public void Number_get_06000681() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000681);

    [SkippableFact]
    public void Number_set_06000682() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000682);

    [SkippableFact]
    public void Type_get_06000683() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000683);

    [SkippableFact]
    public void Type_set_06000684() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000684);

    [SkippableFact]
    public void IsFax_get_06000685() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000685);

    [SkippableFact]
    public void IsFax_set_06000686() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000686);

    [SkippableFact]
    public void IsVoice_get_06000687() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000687);

    [SkippableFact]
    public void IsVoice_set_06000688() => Mechanical.Compare(typeof(global::Quire.VCard.Telephone), 0x06000688);
}
