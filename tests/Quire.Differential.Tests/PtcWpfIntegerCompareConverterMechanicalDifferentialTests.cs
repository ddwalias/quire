namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.IntegerCompareConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.IntegerCompareConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfIntegerCompareConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ConvertBack_06000485() => Mechanical.Compare(typeof(global::Quire.Wpf.IntegerCompareConverter), 0x06000485);

    [SkippableFact]
    public void CompareOperator_get_06000481() => Mechanical.Compare(typeof(global::Quire.Wpf.IntegerCompareConverter), 0x06000481);

    [SkippableFact]
    public void CompareOperator_set_06000482() => Mechanical.Compare(typeof(global::Quire.Wpf.IntegerCompareConverter), 0x06000482);
}
