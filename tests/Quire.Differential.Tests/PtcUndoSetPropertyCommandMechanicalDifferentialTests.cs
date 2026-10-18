namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.SetPropertyCommand</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.SetPropertyCommand.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoSetPropertyCommandMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearValue_060006f1() => Mechanical.Compare(typeof(global::Quire.Undo.SetPropertyCommand), 0x060006f1);

    [SkippableFact]
    public void Redo_060006f2() => Mechanical.Compare(typeof(global::Quire.Undo.SetPropertyCommand), 0x060006f2);

    [SkippableFact]
    public void Value_get_060006ed() => Mechanical.Compare(typeof(global::Quire.Undo.SetPropertyCommand), 0x060006ed);

    [SkippableFact]
    public void Value_set_060006ee() => Mechanical.Compare(typeof(global::Quire.Undo.SetPropertyCommand), 0x060006ee);
}
