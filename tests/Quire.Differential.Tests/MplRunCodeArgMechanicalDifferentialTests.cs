namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.CodeArg</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.CodeArg.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunCodeArgMechanicalDifferentialTests
{
    [SkippableFact]
    public void BegArg_0600034e() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.CodeArg), 0x0600034e);

    [SkippableFact]
    public void EndArg_0600034f() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.CodeArg), 0x0600034f);
}
