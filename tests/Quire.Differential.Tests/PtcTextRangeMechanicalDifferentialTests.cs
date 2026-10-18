namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.TextRange</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.TextRange.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcTextRangeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060002ac() => Mechanical.Compare(typeof(global::Quire.TextRange), 0x060002ac);
}
