namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.UndoableCommand</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.UndoableCommand.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoUndoableCommandMechanicalDifferentialTests
{
    [SkippableFact]
    public void Do_060006fc() => Mechanical.Compare(typeof(global::Quire.Undo.UndoableCommand), 0x060006fc);

    [SkippableFact]
    public void Undo_060006fd() => Mechanical.Compare(typeof(global::Quire.Undo.UndoableCommand), 0x060006fd);

    [SkippableFact]
    public void Redo_060006fe() => Mechanical.Compare(typeof(global::Quire.Undo.UndoableCommand), 0x060006fe);

    [SkippableFact]
    public void Name_get_060006ff() => Mechanical.Compare(typeof(global::Quire.Undo.UndoableCommand), 0x060006ff);
}
