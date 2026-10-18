namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.SymbolicSimpleResultNode</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.SymbolicSimpleResultNode.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreSymbolicSimpleResultNodeMechanicalDifferentialTests
{
    [SkippableFact]
    public void Accept_060001eb() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SymbolicSimpleResultNode), 0x060001eb);

    [SkippableFact]
    public void ctor_060001ec() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SymbolicSimpleResultNode), 0x060001ec);

    [SkippableFact]
    public void Xml_get_060001e7() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SymbolicSimpleResultNode), 0x060001e7);

    [SkippableFact]
    public void Xml_set_060001e8() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SymbolicSimpleResultNode), 0x060001e8);
}
