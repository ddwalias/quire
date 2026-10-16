namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.Code</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.Code.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunCodeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000347() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Code), 0x06000347);
}
