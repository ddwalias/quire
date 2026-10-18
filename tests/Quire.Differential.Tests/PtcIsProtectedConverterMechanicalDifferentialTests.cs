namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.IsProtectedConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.IsProtectedConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcIsProtectedConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000245() => Mechanical.Compare(typeof(global::Quire.IsProtectedConverter), 0x06000245);

    [SkippableFact]
    public void ConvertBack_06000247() => Mechanical.Compare(typeof(global::Quire.IsProtectedConverter), 0x06000247);
}
