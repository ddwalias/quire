namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.CommandScope</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.CommandScope.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoCommandScopeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060006d7() => Mechanical.Compare(typeof(global::Quire.Undo.CommandScope), 0x060006d7);

    [SkippableFact]
    public void ctor_060006d9() => Mechanical.Compare(typeof(global::Quire.Undo.CommandScope), 0x060006d9);
}
