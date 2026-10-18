namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.EmptySimpleResultNode</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.EmptySimpleResultNode.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreEmptySimpleResultNodeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ToString_06000188() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EmptySimpleResultNode), 0x06000188);

    [SkippableFact]
    public void Accept_0600018a() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EmptySimpleResultNode), 0x0600018a);

    [SkippableFact]
    public void ctor_0600018b() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.EmptySimpleResultNode), 0x0600018b);
}
