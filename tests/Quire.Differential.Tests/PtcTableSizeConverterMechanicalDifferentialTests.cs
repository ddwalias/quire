namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.TableSizeConverter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.TableSizeConverter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcTableSizeConverterMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060001b9() => Mechanical.Compare(typeof(global::Quire.TableSizeConverter), 0x060001b9);
}
