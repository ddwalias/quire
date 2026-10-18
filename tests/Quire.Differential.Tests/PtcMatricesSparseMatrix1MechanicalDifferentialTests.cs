namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Matrices.SparseMatrix`1</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Matrices.SparseMatrix`1.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcMatricesSparseMatrix1MechanicalDifferentialTests
{
    [SkippableFact]
    public void PopulatedCount_get_06000eb6() => Mechanical.Compare(typeof(global::Quire.Matrices.SparseMatrix<>), 0x06000eb6);

    [SkippableFact]
    public void PopulatedPositions_get_06000eb7() => Mechanical.Compare(typeof(global::Quire.Matrices.SparseMatrix<>), 0x06000eb7);

    [SkippableFact]
    public void Changed_add_06000eb2() => Mechanical.Compare(typeof(global::Quire.Matrices.SparseMatrix<>), 0x06000eb2);

    [SkippableFact]
    public void Changed_remove_06000eb3() => Mechanical.Compare(typeof(global::Quire.Matrices.SparseMatrix<>), 0x06000eb3);

    [SkippableFact]
    public void Position_get_0600138d() => Mechanical.Compare(typeof(global::Quire.Matrices.SparseMatrix<>), 0x0600138d);

    [SkippableFact]
    public void Position_set_0600138e() => Mechanical.Compare(typeof(global::Quire.Matrices.SparseMatrix<>), 0x0600138e);
}
