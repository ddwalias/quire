namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Serialization.TextRangeExtended</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Serialization.TextRangeExtended.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcSerializationTextRangeExtendedMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000805() => Mechanical.Compare(typeof(global::Quire.Serialization.TextRangeExtended), 0x06000805);
}
