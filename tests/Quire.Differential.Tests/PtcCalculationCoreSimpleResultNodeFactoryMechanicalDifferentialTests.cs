namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Calculation.Core.SimpleResultNodeFactory</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Calculation.Core.SimpleResultNodeFactory.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests
{
    [SkippableFact]
    public void CreateEmptyResultNode_060001ed() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001ed);

    [SkippableFact]
    public void CreateRealResultNode_060001ee() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001ee);

    [SkippableFact]
    public void CreateImaginaryResultNode_060001f0() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001f0);

    [SkippableFact]
    public void CreateComplexResultNode_060001f2() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001f2);

    [SkippableFact]
    public void CreateStringResultNode_060001f4() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001f4);

    [SkippableFact]
    public void CreateDisplayMatrixResultNode_060001f7() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001f7);

    [SkippableFact]
    public void CreateEagerMatrixResultNode_060001f8() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001f8);

    [SkippableFact]
    public void CreateLazyMatrixResultNode_060001f9() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001f9);

    [SkippableFact]
    public void CreateUnitedValueResultNode_060001fa() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001fa);

    [SkippableFact]
    public void ctor_060001fc() => Mechanical.Compare(typeof(global::Quire.Calculation.Core.SimpleResultNodeFactory), 0x060001fc);
}
