namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.CodeFunction</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.CodeFunction.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunCodeFunctionMechanicalDifferentialTests
{
    [SkippableFact]
    public void EmitCode_06000349() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.CodeFunction), 0x06000349);
}
