namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.Code</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.Code.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunCodeMechanicalDifferentialTests
{
    [SkippableFact]
    public void EmitCode_06000344() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Code), 0x06000344);

    [SkippableFact]
    public void BegArg_06000345() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Code), 0x06000345);

    [SkippableFact]
    public void EndArg_06000346() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Code), 0x06000346);

    [SkippableFact]
    public void ctor_06000347() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.Code), 0x06000347);
}
