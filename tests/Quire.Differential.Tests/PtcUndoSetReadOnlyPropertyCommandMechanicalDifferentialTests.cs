namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.SetReadOnlyPropertyCommand</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.SetReadOnlyPropertyCommand.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoSetReadOnlyPropertyCommandMechanicalDifferentialTests
{
    [SkippableFact]
    public void ClearValue_060006f9() => Mechanical.Compare(typeof(global::Quire.Undo.SetReadOnlyPropertyCommand), 0x060006f9);

    [SkippableFact]
    public void Value_get_060006f7() => Mechanical.Compare(typeof(global::Quire.Undo.SetReadOnlyPropertyCommand), 0x060006f7);

    [SkippableFact]
    public void Value_set_060006f8() => Mechanical.Compare(typeof(global::Quire.Undo.SetReadOnlyPropertyCommand), 0x060006f8);
}
