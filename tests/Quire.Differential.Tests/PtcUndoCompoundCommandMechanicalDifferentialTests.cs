namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Undo.CompoundCommand</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Undo.CompoundCommand.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcUndoCompoundCommandMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060006e0() => Mechanical.Compare(typeof(global::Quire.Undo.CompoundCommand), 0x060006e0);

    [SkippableFact]
    public void ctor_060006e1() => Mechanical.Compare(typeof(global::Quire.Undo.CompoundCommand), 0x060006e1);

    [SkippableFact]
    public void Name_get_060006de() => Mechanical.Compare(typeof(global::Quire.Undo.CompoundCommand), 0x060006de);

    [SkippableFact]
    public void Name_set_060006df() => Mechanical.Compare(typeof(global::Quire.Undo.CompoundCommand), 0x060006df);
}
